using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace MyGame;

public class BlockProperties
{
    public string name { get; set; }
    public int id { get; set; }
    public string texturePath { get; set; }
    public bool isSolid { get; set; }
    public bool animated { get; set; }
    public int animationFrameCount { get; set; }
    public int animationSpeed { get; set; }
    public int tileSize { get; set; }

    public static BlockProperties FromJson(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string jsonString = File.ReadAllText(filePath);

        BlockProperties? props = JsonSerializer.Deserialize<BlockProperties>(jsonString, options);

        if (props == null)
        {
            throw new Exception($"Failed to load block properties from {filePath}");
        }

        return props;
    }
}