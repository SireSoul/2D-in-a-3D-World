namespace MyGame;

public static class ItemRegistry
{
    public static void RegisterAll()
    {
        ItemManager.Register(new Stone());
    }
}