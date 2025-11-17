using System.Numerics;
using Raylib_cs;

namespace MyGame;

public static class BlockManager
{
    private static readonly Dictionary<int, Block> registry = new();

    public static void Register(Block block)
    {
        if (registry.ContainsKey(block.Id))
            throw new Exception($"Duplicate block ID {block.Id}");

        block.LoadTexture();
        registry[block.Id] = block;

        Console.WriteLine($"Registered Block: {block.Name} (ID {block.Id})");
    }

    public static Block Create(int id, Vector2 pos)
    {
        if (!registry.TryGetValue(id, out Block? template) || template == null)
            throw new Exception($"Block ID {id} not registered!");

        Block newBlock = (Block)Activator.CreateInstance(template.GetType())!;
        newBlock.LoadTexture();
        newBlock.Position = pos;

        return newBlock;
    }

    public static Block GetTemplate(int id)
    {
        if (!registry.TryGetValue(id, out Block? block) || block == null)
            throw new Exception($"Block ID {id} not registered!");

        return block;
    }
}