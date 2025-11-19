using System.Numerics;
using Raylib_cs;

namespace MyGame;

public static class BlockManager
{
    private static readonly Dictionary<int, Block> registry = new();

    // New: pending rotation per block id
    private static readonly Dictionary<int, int> pendingRotationIndices = new();

    public static void Register(Block block)
    {
        if (registry.ContainsKey(block.Id))
            throw new Exception($"Duplicate block ID {block.Id}");

        // Load texture ONCE for the template block
        block.LoadTexture();
        registry[block.Id] = block;

        // Default pending rotation = 0
        pendingRotationIndices[block.Id] = 0;

        Console.WriteLine($"Registered Block: {block.Name} (ID {block.Id})");
    }

    public static Block Create(int id, Vector2 pos)
    {
        if (!registry.TryGetValue(id, out Block? template) || template == null)
            throw new Exception($"Block ID {id} not registered!");

        // Create a NEW instance of the same type
        Block newBlock = (Block)Activator.CreateInstance(template.GetType())!;

        // Share the same texture (don’t reload)
        newBlock.Texture = template.Texture;
        newBlock.Position = pos;
        newBlock.Hardness = newBlock.MaxHardness;

        // Apply pending rotation for this block type
        if (newBlock.Rotatable)
        {
            int pendingIndex = GetPendingRotationIndex(id);
            newBlock.SetRotationIndex(pendingIndex);
        }

        return newBlock;
    }

    public static Block GetTemplate(int id)
    {
        if (!registry.TryGetValue(id, out Block? block) || block == null)
            throw new Exception($"Block ID {id} not registered!");

        return block;
    }

    // --- NEW HELPERS ---

    private static int GetPendingRotationIndex(int id)
    {
        if (pendingRotationIndices.TryGetValue(id, out int idx))
            return idx;
        return 0;
    }

    public static void CyclePendingRotation(int id)
    {
        if (!registry.TryGetValue(id, out Block? template) || template == null)
            return;

        if (!template.Rotatable || template.RotationOrder.Length == 0)
            return;

        int current = GetPendingRotationIndex(id);
        current = (current + 1) % template.RotationOrder.Length;
        pendingRotationIndices[id] = current;

        Console.WriteLine($"Next {template.Name} rotation index: {current}");
    }
}