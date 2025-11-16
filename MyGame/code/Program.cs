using MyGame;
using Raylib_cs;

public static class Program
{
    public static void Main()
    {
        const int screenW = 1280;
        const int screenH = 720;

        Raylib.InitWindow(screenW, screenH, "My Game");
        Raylib.SetTargetFPS(60);

        Game game = new(screenW, screenH);

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            game.Update(dt);
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}