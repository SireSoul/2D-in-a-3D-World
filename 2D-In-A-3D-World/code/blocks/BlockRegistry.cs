namespace MyGame;

public static class BlockRegistry
{
    public static void RegisterAll()
    {
        BlockManager.Register(new ShaftBlock());
    }
}