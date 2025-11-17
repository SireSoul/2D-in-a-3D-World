using System.Numerics;
using MyGame;
using Raylib_cs;

public class Inventory
{
    public ItemStack?[] Slots;
    public int Rows = 4;
    public int Columns = 12;
    public int HotbarRows = 1;
    public int TotalSlots => (Rows + HotbarRows) * Columns;
    public int SelectedSlot = 0;
    public ItemStack? HeldStack = null;
    private bool clickHandled = false;
    private const int SlotPaddingY = 10;
    private float heldDisplayCount = 0;
    private const float heldCountLerpSpeed = 24f;

    public Inventory()
    {
        Slots = new ItemStack?[TotalSlots];
    }

    public bool AddItem(Item item, int count = 1)
    {
        int remaining = count;
        for (int i = 0; i < Slots.Length; i++)
        {
            var slot = Slots[i];
            if (slot == null) continue;

            if (slot.Item.Id == item.Id && !slot.IsFull())
            {
                int space = slot.MaxStackSize - slot.Count;

                int toAdd = Math.Min(remaining, space);
                slot.Add(toAdd);

                remaining -= toAdd;

                if (remaining <= 0)
                    return true;
            }
        }

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i] == null)
            {
                int toInsert = Math.Min(remaining, item.MaxStack);
                Slots[i] = new ItemStack(item, toInsert);

                remaining -= toInsert;

                if (remaining <= 0)
                    return true;
            }
        }
        return false;
    }

    public bool RemoveItem(int slot, int amount)
    {
        if (slot < 0 || slot >= Slots.Length) return false;
        if (Slots[slot] == null) return false;

        Slots[slot]!.Remove(amount);

        if (Slots[slot]!.Count == 0)
            Slots[slot] = null;

        return true;
    }

    public void DrawInventory(Player player)
    {
        float invWidth  = UIManager.InventoryTexture.Width * UIManager.InventoryScale;
        float invHeight = UIManager.InventoryTexture.Height * UIManager.InventoryScale;

        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight();

        float invX = (screenW - invWidth) / 2f;
        float invY = (screenH - invHeight) / 2f;

        Vector2 invOrigin = new Vector2(invX, invY);
        Raylib.DrawTextureEx(
            UIManager.InventoryTexture,
            invOrigin,
            0f,
            UIManager.InventoryScale,
            Color.White
        );
        Raylib.SetTextureFilter(UIManager.InventoryTexture, TextureFilter.Point);

        int columns = player.playerInventory.Columns;
        int rows = player.playerInventory.Rows;
        int hotbarRows = player.playerInventory.HotbarRows;

        int baseSlotSize = 16;
        float slotSize = baseSlotSize * UIManager.InventoryScale;

        float hotbarExtraOffset = 4 * UIManager.InventoryScale;
        float hotbarY = invY + SlotPaddingY + (rows * slotSize) + hotbarExtraOffset;

        for (int i = 0; i < player.playerInventory.TotalSlots; i++)
        {
            var stack = player.playerInventory.Slots[i];
            if (stack == null) continue;

            int col = i % columns;
            int row = i / columns;

            float drawX = invX + col * slotSize + 44;
            float drawY;
            if (row < rows)
                drawY = invY + SlotPaddingY + row * slotSize + 62;
            else
                drawY = hotbarY + 48;

            Raylib.DrawTextureEx(
                stack.Item.Texture,
                new Vector2(drawX, drawY),
                0f,
                UIManager.InventoryScale - 1,
                Color.White
            );

            if (stack.Count > 1)
            {
                Raylib.DrawText(
                    stack.Count.ToString(),
                    (int)(drawX + 28),
                    (int)(drawY + 36),
                    16,
                    Color.White
                );
            }
        }

        Vector2 mouse = Raylib.GetMousePosition();
        mouse.Y -= 64;
        mouse.X -= 48;

        float localX = mouse.X - invX;
        float localY = mouse.Y - invY;

        int hoverCol = (int)(localX / slotSize);
        int hoverRow = (int)(localY / slotSize);

        bool inside =
            localX >= 0 && localY >= 0 &&
            hoverCol >= 0 && hoverCol < columns &&
            hoverRow >= 0 && hoverRow < (rows + hotbarRows);

        if (inside)
        {
            hoverCol = (int)(localX / slotSize);
            hoverRow = (int)(localY / slotSize);

            float selX = invX + hoverCol * slotSize;
            float selY;
            if (hoverRow < rows) {
                selY = invY + hoverRow * slotSize;
            }
            else {
                selY = hotbarY - 22;
            }

            Raylib.DrawTextureEx(
                UIManager.InventorySelectionTexture,
                new Vector2(selX + 36, selY + 64),
                0f,
                UIManager.InventoryScale,
                Color.White
            );

            // Smooth count for held stack ONLY
            if (HeldStack != null)
            {
                float target = HeldStack.Count;
                heldDisplayCount = Utils.Lerp(heldDisplayCount, target, Raylib.GetFrameTime() * heldCountLerpSpeed);

                if (MathF.Abs(heldDisplayCount - target) < 0.1f)
                    heldDisplayCount = target;
            }
            else
            {
                heldDisplayCount = 0; // reset when no item held
            }

            int hoverIndex = hoverRow * columns + hoverCol;
            bool leftPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            bool rightPressed = Raylib.IsMouseButtonPressed(MouseButton.Right);

            if (leftPressed && !player.playerInventory.clickHandled)
            {
                var slots = player.playerInventory.Slots;
                var held = player.playerInventory.HeldStack;
                var slotStack = slots[hoverIndex];

                if (held == null && slotStack != null)
                {
                    player.playerInventory.HeldStack = slotStack;
                    slots[hoverIndex] = null;
                } else if (held != null && slotStack == null)
                {
                    slots[hoverIndex] = held;
                    player.playerInventory.HeldStack = null;
                } else if (held != null && slotStack != null && slotStack.Item.Id == held.Item.Id && !slotStack.IsFull())
                {
                    int space = slotStack.MaxStackSize - slotStack.Count;
                    int move = Math.Min(space, held.Count);

                    slotStack.Add(move);
                    held.Remove(move);

                    if (held.Count == 0)
                        player.playerInventory.HeldStack = null;
                } else if (held != null && slotStack != null)
                {
                    player.playerInventory.Slots[hoverIndex] = held;
                    player.playerInventory.HeldStack = slotStack;
                }

                player.playerInventory.clickHandled = true;
            } else if (Raylib.IsMouseButtonDown(MouseButton.Right) && !player.playerInventory.clickHandled && !Raylib.IsKeyDown(KeyboardKey.V))
            {
                var slots = player.playerInventory.Slots;
                var held = player.playerInventory.HeldStack;
                var slotStack = slots[hoverIndex];

                if (slotStack == null)
                {
                    player.playerInventory.clickHandled = true;
                    return;
                }
                if (held == null)
                {
                    player.playerInventory.HeldStack = new ItemStack(slotStack.Item, 1);

                    slotStack.Remove(1);
                    if (slotStack.Count == 0)
                        slots[hoverIndex] = null;

                    player.playerInventory.clickHandled = true;
                    return;
                }
                if (held.Item.Id == slotStack.Item.Id && !held.IsFull())
                {
                    held.Add(1);
                    slotStack.Remove(1);

                    if (slotStack.Count == 0)
                        slots[hoverIndex] = null;

                    player.playerInventory.clickHandled = true;
                    return;
                }
                player.playerInventory.clickHandled = true;
            } else if (Raylib.IsMouseButtonDown(MouseButton.Right) && !player.playerInventory.clickHandled && Raylib.IsKeyDown(KeyboardKey.V))
            {
                var slots = player.playerInventory.Slots;
                var held = player.playerInventory.HeldStack;
                var slotStack = slots[hoverIndex];

                if (slotStack == null)
                {
                    player.playerInventory.clickHandled = true;
                    return;
                }
                if (held == null)
                {
                    player.playerInventory.HeldStack = new ItemStack(slotStack.Item, 5);

                    slotStack.Remove(5);
                    if (slotStack.Count == 0)
                        slots[hoverIndex] = null;

                    player.playerInventory.clickHandled = true;
                    return;
                }
                if (held.Item.Id == slotStack.Item.Id && !held.IsFull())
                {
                    held.Add(5);
                    slotStack.Remove(5);

                    if (slotStack.Count == 0)
                        slots[hoverIndex] = null;

                    player.playerInventory.clickHandled = true;
                    return;
                }
                player.playerInventory.clickHandled = true;
            }

            if (!Raylib.IsMouseButtonDown(MouseButton.Left)) {
                player.playerInventory.clickHandled = false;
            }
        }
        
        if (player.playerInventory.HeldStack != null)
        {
            var held = player.playerInventory.HeldStack;

            Vector2 mousePos = Raylib.GetMousePosition();
            float scale = UIManager.InventoryScale - 1;

            Raylib.DrawTextureEx(
                held.Item.Texture,
                mousePos - new Vector2(8, 8),
                0f,
                scale,
                Color.White
            );

            if (held.Count > 1)
            {
                Raylib.DrawText(
                    ((int)heldDisplayCount).ToString(),
                    (int)(mousePos.X + 10),
                    (int)(mousePos.Y + 10),
                    16,
                    Color.White
                );
            }
        }
    }
}