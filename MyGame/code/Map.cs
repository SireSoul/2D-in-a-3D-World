using System.Numerics;
using Raylib_cs;
using TiledSharp;

namespace MyGame;

public class Map
{
    private TmxMap tmxMap;
    private Texture2D mapTileset;
    private int tileWidth;
    private int tileHeight;
    private int tilesetTilesPerRow;
    public int PixelWidth { get; private set; }
    public int PixelHeight { get; private set; }

    public Map(String mapPath)
    {
        tmxMap = new TmxMap(mapPath);

        tileWidth = tmxMap.TileWidth;
        tileHeight = tmxMap.TileHeight;

        string tilesetImage = tmxMap.Tilesets[0].Image.Source;
        string tilesetPath = tilesetImage;
        mapTileset = Raylib.LoadTexture(tilesetPath);
        
        tilesetTilesPerRow = mapTileset.Width / tileWidth;

        PixelWidth = tmxMap.Width * tileWidth;
        PixelHeight = tmxMap.Height * tileHeight;
    }

    public void Draw()
    {
        if (mapTileset.Width == 0) {return;}

        foreach (var layer in tmxMap.Layers)
        {
            for (int i = 0; i < layer.Tiles.Count; i++)
            {
                int gid = layer.Tiles[i].Gid;

                if (gid == 0) {continue;}

                int tileX = i % tmxMap.Width;
                int tileY = i / tmxMap.Width;

                int tileId = gid - 1;

                int srcX = (tileId % tilesetTilesPerRow) * tileWidth;
                int srcY = (tileId / tilesetTilesPerRow) * tileHeight;

                Rectangle src = new Rectangle(srcX, srcY, tileWidth, tileHeight);
                Vector2 destPos = new Vector2(tileX * tileWidth, tileY * tileHeight);

                Raylib.DrawTextureRec(mapTileset, src, destPos, Color.White);
            }
        }
    }
}