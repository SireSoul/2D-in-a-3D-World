using Raylib_cs;
using System.Numerics;

namespace MyGame;

public static class UIManager
{
    public static Texture2D HotbarTexture;
    public static Texture2D InventoryTexture;
    public static Texture2D HotbarSelectionTexture;
    public static Texture2D InventorySelectionTexture;
    public static float InventoryScale = 4f;
    public static float hotbarScale = 4f;
    public static float HotbarSelectionScale = 4f;
    public static float hotbarItemScale = 3f;

    public static bool InventoryOpen = false;

    public static void Load()
    {
        HotbarTexture = Raylib.LoadTexture("assets/ui/hotbar.png");
        InventoryTexture = Raylib.LoadTexture("assets/ui/inventory.png");
        HotbarSelectionTexture = Raylib.LoadTexture("assets/ui/hotbar_selection.png");
        InventorySelectionTexture = Raylib.LoadTexture("assets/ui/hotbar_selection.png");
    }

    public static void Update(Player player)
    {
        // Toggle inventory
        if (Raylib.IsKeyPressed(KeyboardKey.E) || (Raylib.IsKeyPressed(KeyboardKey.Escape) && InventoryOpen == true))
        {
            InventoryOpen = !InventoryOpen;
        }
    }

    public static void Draw(Player player)
    {
        HotBar.DrawHotbar(player);

        if (InventoryOpen) {player.playerInventory.DrawInventory(player);}
    }   
}