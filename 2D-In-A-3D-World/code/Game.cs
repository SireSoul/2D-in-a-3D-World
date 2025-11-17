namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Game
{
    private Camera camera;
    private Player player;
    private Map map;
    private Block?[,] world;
    RenderTexture2D worldTarget;
    public float timeOfDay = 0f;
    Shader dayNightShader;
    int brightnessLoc;
    private float daylight = 1f;

    public Game(int width, int height)
    {
        world = new Block[73, 55];

        camera = new Camera(
            new Vector2(width / 2f, height / 2f),
            1f
        );
        BlockRegistry.RegisterAll();
        ItemRegistry.RegisterAll();

        map = new Map("assets/maps/spring_map.tmx");
        player = new Player(100, 100, map);
        UIManager.Load();
        worldTarget = Raylib.LoadRenderTexture(width, height);
        dayNightShader = Raylib.LoadShader(null, "assets/shaders/daynight.fs");
        brightnessLoc = Raylib.GetShaderLocation(dayNightShader, "brightness");
    }

    public void Update(float dt)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Slash) || Raylib.IsKeyPressed(KeyboardKey.Grave))
        {
            GameConsole.Toggle();
        }
        if (GameConsole.IsOpen)
        {
            GameConsole.Update(player);
            return;
        }
        player.Update(dt);
        UIManager.Update(player);
        camera.HandleZoomInput(dt);
        camera.Follow(player);
        
        if (Raylib.IsMouseButtonDown(MouseButton.Right))
        {
            Vector2 mouse = Raylib.GetMousePosition();
            Vector2 worldPos = Raylib.GetScreenToWorld2D(mouse, camera.ToRaylibCam());
            
            int tileX = (int)(worldPos.X / 16);
            int tileY = (int)(worldPos.Y / 16);

            if (tileX >= 0 && tileX < 73 && tileY >= 0 && tileY < 55)
            {
                if (world[tileX, tileY] == null)
                {
                    Block block = BlockManager.Create(1, new Vector2(tileX * 16, tileY * 16));
                    world[tileX, tileY] = block;

                    block.OnPlace();
                }
            }
        }

        timeOfDay += dt * 0.02f;
        if (timeOfDay > 1f) timeOfDay -= 1f;
        daylight = MathF.Sin(timeOfDay * MathF.PI * 2) * 0.5f + 0.5f;
    }

    public void Draw()
    {
        Raylib.BeginTextureMode(worldTarget);
        Raylib.ClearBackground(Color.Black);

        camera.Begin();
        map.Draw();
        player.Draw(camera.Zoom);
        player.DrawHeldItem();
        camera.End();

        Raylib.EndTextureMode();
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginShaderMode(dayNightShader);
        Raylib.SetShaderValue(dayNightShader, brightnessLoc, daylight, ShaderUniformDataType.Float);

        Raylib.DrawTextureRec(
            worldTarget.Texture,
            new Rectangle(0, 0, worldTarget.Texture.Width, -worldTarget.Texture.Height),
            new Vector2(0, 0),
            Color.White
        );

        Raylib.EndShaderMode();

        UIManager.Draw(player);
        GameConsole.Draw();

        Raylib.EndDrawing();
    }
}