namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Game
{
    private Camera camera;
    private Player player;
    private Map map;
    public Block?[,] world;

    private DayNightClock Clock;
    RenderTexture2D worldTarget;

    public float timeOfDay = 0f;
    private float daylight = 1f;

    Shader dayNightShader;
    int brightnessLoc;
    public bool IsTileSolid(int x, int y)
    {
        if (x < 0 || y < 0 || x >= 73 || y >= 55) 
            return false; // out of bounds = not solid

        Block? b = world[x, y];
        return b != null && b.IsSolid;
    }

    public Game(int width, int height)
    {
        world = new Block[73, 55];

        camera = new Camera(
            new Vector2(width / 2f, height / 2f),
            2.5f
        );

        Clock = new DayNightClock(6, 0);

        BlockRegistry.RegisterAll();
        ItemRegistry.RegisterAll();

        map = new Map("assets/maps/spring_map.tmx");
        player = new Player(width / 2f, height / 2f, map, this);

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

        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            BlockManager.CyclePendingRotation(1);
        }

        player.Update(dt);
        UIManager.Update(player);
        camera.HandleZoomInput(dt);
        camera.Follow(player);
        Clock.Update(dt, minutesPerRealSecond: 1f);

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

        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            Vector2 mouse = Raylib.GetMousePosition();
            Vector2 worldPos = Raylib.GetScreenToWorld2D(mouse, camera.ToRaylibCam());

            int tileX = (int)(worldPos.X / 16);
            int tileY = (int)(worldPos.Y / 16);
            Block b = world[tileX, tileY];

            if (tileX >= 0 && tileX < 73 && tileY >= 0 && tileY < 55)
            {

                if (b != null)
                {
                    b.IsBeingMined = true;
                    b.MineTimer += dt;
                    if (b.MineTimer >= b.MineDelay)
                    {
                        b.MineTimer = 0f;
                        b.Hardness -= 1;

                        if (b.Hardness <= 0)
                        {
                            b.OnBreak();
                            world[tileX, tileY] = null;
                        }
                    }
                }
            }
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            for (int x = 0; x < 73; x++)
            {
                for (int y = 0; y < 55; y++)
                {
                    Block b = world[x, y];
                    if (b != null)
                    {
                        b.IsBeingMined = false;
                        b.MineTimer = 0f;
                    }
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

        List<Block> drawList = new List<Block>();

        for (int x = 0; x < 73; x++)
        {
            for (int y = 0; y < 55; y++)
            {
                Block b = world[x, y];
                if (b != null)
                {
                    drawList.Add(b);
                    b.Update(Raylib.GetFrameTime());
                    b.Draw();
                }
            }
        }

        drawList.Sort((a, b) =>
        {
        float ay = a.Position.Y + a.TileSize;
        float by = b.Position.Y + b.TileSize;
        return ay.CompareTo(by);
        });

        float playerFeet = player.Position.Y + player.Height / 2;

        foreach (var block in drawList)
        {
        float blockBottom = block.Position.Y + block.TileSize;

        if (blockBottom < playerFeet)
        {
            block.Draw();
        } else
        {
            break;
        }
        }

        player.Draw(camera.Zoom);
        player.DrawHeldItem();

        foreach (var block in drawList)
        {
        float blockBottom = block.Position.Y + block.TileSize;

        if (blockBottom >= playerFeet)
        {
            block.Draw();
        }
        }

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
        Clock.Draw();
        GameConsole.Draw();

        Raylib.EndDrawing();
    }
}