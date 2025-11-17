using Raylib_cs;
using System.Numerics;

namespace MyGame;

public static class ItemManager
{
    private static readonly Dictionary<int, Item> registry = new();
    private static readonly Dictionary<int, Texture2D> textures = new();

    public static void Register(Item item)
    {
        if (registry.ContainsKey(item.Id))
            throw new Exception($"Duplicate item ID: {item.Id}");

        item.LoadTexture();
        registry[item.Id] = item;
        textures[item.Id] = item.Texture;

        Console.WriteLine($"Registered Item: {item.Name} (ID {item.Id})");
    }

    public static Item Create(int id, Vector2 pos)
    {
        if (!registry.TryGetValue(id, out Item? template) || template == null)
            throw new Exception($"Item ID {id} not registered!");

        Item item = (Item)Activator.CreateInstance(template.GetType())!;
        item.LoadTexture();
        item.Position = pos;

        return item;
    }

    public static Texture2D GetTexture(int id) => textures[id];

    public static void loadItems()
    {
        ItemManager.Register(new Stone());
    }

    public static Item? FindByName(string name)
    {
        name = name.ToLower();

        foreach (var kv in registry)
        {
            if (kv.Value.Name.ToLower() == name)
                return kv.Value;
        }

        return null;
    }
}