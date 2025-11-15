namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Game
{
    private Camera camera;
    private Player player;

    public Game(int width, int height)
    {
        player = new Player(100, 100);

        // Camera offset = center of window
        camera = new Camera(
            new Vector2(width / 2f, height / 2f),
            1f
        );
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
        player.Draw();
        camera.End();

        Raylib.EndDrawing();
    }
}