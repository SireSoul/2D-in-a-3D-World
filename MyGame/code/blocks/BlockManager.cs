using Raylib_cs;
using System.Numerics;
using System.Text.Json;
namespace MyGame;

public static class BlockManager
{
    private static Dictionary<int, BlockProperties> properties = new();
    private static Dictionary<int, Texture2D> textures = new();

    public static void LoadAllBlocks(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*.json");

        foreach (var file in files)
        {
            BlockProperties props = BlockProperties.FromJson(file);

            int id = props.id.GetHashCode();

            properties[id] = props;
            Texture2D texture = Raylib.LoadTexture(props.texturePath);
            textures[id] = texture;

            Console.WriteLine($"Loaded Block: {props.name} (ID: {props.id}), Texture: {props.texturePath}, Tile Size: {props.tileSize}");
        }
    }

    public static Block CreateBlock(int id, float x, float y)
    {
        BlockProperties props = GetProperties(id);
        Texture2D tex = GetTexture(id);

        return new Block(props, tex, new Vector2(x, y));
    }

    public static BlockProperties GetProperties(int id)
    {
        return properties[id];
    }

    public static Texture2D GetTexture(int id)
    {
        return textures[id];
    }
}