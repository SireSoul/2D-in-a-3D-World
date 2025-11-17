using System.Numerics;
using MyGame;
using Raylib_cs;

public class HotBar
{
    public ItemStack?[] Slots = new ItemStack?[12];
    public int SelectedSlot = 0;

    public static void DrawHotbar(Player player)
    {
        float barWidth = UIManager.HotbarTexture.Width  * UIManager.hotbarScale;
        float barHeight = UIManager.HotbarTexture.Height * UIManager.hotbarScale;
        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight();

        Vector2 pos = new Vector2(
            screenW / 2 - UIManager.HotbarTexture.Width / 2,
            screenH - UIManager.HotbarTexture.Height - 10
        );

        float barX = (screenW - barWidth) / 2f;
        float barY = screenH - barHeight;

        Raylib.DrawTextureEx(UIManager.HotbarTexture, new Vector2(barX, barY), 0f, UIManager.hotbarScale, Color.White);
        Raylib.SetTextureFilter(UIManager.HotbarTexture, TextureFilter.Point);

        for (int slot = 0; slot < 12; slot++)
        {
            var itemStack = player.Hotbar.Slots[slot];
            if (itemStack == null) continue;

            float baseSlotSize = 32f;  // base pixel size of each hotbar slot
            float slotSize = baseSlotSize * UIManager.hotbarScale;

            // Item size — match inventory scale OR set hotbar-specific scale
            float itemScale = UIManager.hotbarItemScale;  // you define this
            if (itemScale <= 0) itemScale = UIManager.hotbarScale - 0.5f;

            // Compute final size of item texture
            float itemSize = 16 * itemScale;

            // Center item inside slot
            float x = barX + slot * slotSize / 2;
            float y = barY + (slotSize - itemSize) / 2;

            // Draw item
            Raylib.DrawTextureEx(
                itemStack.Item.Texture,
                new Vector2(x + 18, y - 20),
                0f,
                itemScale,
                Color.White
            );

            if (itemStack.Count > 1)
            {
                Raylib.DrawText(
                    itemStack.Count.ToString(),
                    (int)(x + 36),
                    (int)(y + 8),
                    (int)(5 * UIManager.hotbarScale),
                    Color.White
                );
            }
        }
        DrawHotbarSelection(player, pos);
    }

    public static void DrawHotbarSelection(Player player, Vector2 hotbarPos)
    {
        int selected = player.Hotbar.SelectedSlot;

        float slotSize = 64f;

        float x = hotbarPos.X + selected * slotSize;
        float y = hotbarPos.Y;

        Raylib.DrawTextureEx(UIManager.HotbarSelectionTexture, new Vector2(x - 285, y - 35), 0f, UIManager.HotbarSelectionScale, Color.White);
    }

    public void HandleHotbarNumberKeys(HotBar hotbar)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.One))  hotbar.SelectedSlot = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.Two))  hotbar.SelectedSlot = 1;
        if (Raylib.IsKeyPressed(KeyboardKey.Three)) hotbar.SelectedSlot = 2;
        if (Raylib.IsKeyPressed(KeyboardKey.Four))  hotbar.SelectedSlot = 3;
        if (Raylib.IsKeyPressed(KeyboardKey.Five))  hotbar.SelectedSlot = 4;
        if (Raylib.IsKeyPressed(KeyboardKey.Six))   hotbar.SelectedSlot = 5;
        if (Raylib.IsKeyPressed(KeyboardKey.Seven)) hotbar.SelectedSlot = 6;
        if (Raylib.IsKeyPressed(KeyboardKey.Eight)) hotbar.SelectedSlot = 7;
        if (Raylib.IsKeyPressed(KeyboardKey.Nine))  hotbar.SelectedSlot = 8;
        if (Raylib.IsKeyPressed(KeyboardKey.Zero))  hotbar.SelectedSlot = 9;
        if (Raylib.IsKeyPressed(KeyboardKey.Minus)) hotbar.SelectedSlot = 10;
        if (Raylib.IsKeyPressed(KeyboardKey.Equal)) hotbar.SelectedSlot = 11;
    }
}