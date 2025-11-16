namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Game
{
    private Camera camera;
    private Player player;
    private Map map;

    public Game(int width, int height)
    {

        // Camera offset = center of window
        camera = new Camera(
            new Vector2(width / 2f, height / 2f),
            1f
        );
        BlockManager.LoadAllBlocks("assets/blocks/json");

        map = new Map("assets/maps/spring_map.tmx");
        player = new Player(100, 100, map);
    }

    public void Update(float dt)
    {
        player.Update(dt);
        camera.HandleZoomInput(dt);
        camera.Follow(player);
    }

    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        camera.Begin();
        map.Draw();
        player.Draw(camera.Zoom);
        camera.End();

        Raylib.EndDrawing();
    }
}