using MyGame;
using Raylib_cs;

public static class Program
{
    public static void Main()
    {
        const int screenW = 1280;
        const int screenH = 720;

        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(screenW, screenH, "2D In A 3D World");
        Raylib.SetExitKey(KeyboardKey.Null);
        Raylib.SetTargetFPS(60);

        Game game = new(screenW, screenH);

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            if (Raylib.IsKeyPressed(KeyboardKey.F11))
            {
                Raylib.ToggleFullscreen();
            }

            game.Update(dt);
            game.Draw();
        }

        Raylib.CloseWindow();
    }
}