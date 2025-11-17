namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Player {
    public Vector2 Position;
    public float Width;
    public float Height;
    public Vector2 Velocity;
    Vector2 move = Vector2.Zero;

    public float Speed = 75f;
    public float Size = 16f;
    private Map map;
    public float X { get; set; }
    public float Y { get; set; }
    public Inventory playerInventory;
    public HotBar Hotbar { get; set; }
    public float HotbarScrollSpeed = 0.5f;
    public Texture2D IdleTexture;
    public Texture2D WalkTexture;
    private int frameWidth = 32;
    private int frameHeight = 32;

    private int idleFrames = 4;
    private int walkFrames = 6;
    private float animationTimer = 0f;
    private float frameSpeed = 0.075f;
    private int currentFrame = 0;
    private int currentDirection = 2;

    public Player(float x, float y, Map gameMap)
    {
        Position = new Vector2(x, y);
        map = gameMap;
        X = x;
        Y = y;
        playerInventory = new Inventory();
        Hotbar = new HotBar();
        IdleTexture = Raylib.LoadTexture("assets/player/Idle.png");
        WalkTexture = Raylib.LoadTexture("assets/player/Walk.png");

        Width = frameWidth;
        Height = frameHeight;
    }

    public void Update(float dt)
    {
        SyncHotbar();
        bool moving = false;
        Vector2 move = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W)) { move.Y -= 1; currentDirection = 1; }
        if (Raylib.IsKeyDown(KeyboardKey.S)) { move.Y += 1; currentDirection = 0; }
        if (Raylib.IsKeyDown(KeyboardKey.A)) { move.X -= 1; currentDirection = 3; }
        if (Raylib.IsKeyDown(KeyboardKey.D)) { move.X += 1; currentDirection = 2; }

        moving = move != Vector2.Zero;

        if (moving)
        {
            move = Vector2.Normalize(move);
            Position += move * Speed * dt;
        }


        Texture2D activeTexture = moving ? WalkTexture : IdleTexture;
        int frameCount = moving ? walkFrames : idleFrames;

        if (moving)
        {
            animationTimer += dt;
            if (animationTimer >= frameSpeed)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % frameCount;
            }
        }
        else
        {
            currentFrame = 0;
        }

        float scroll = Raylib.GetMouseWheelMove();
        if (scroll != 0)
        {
            int s = Hotbar.SelectedSlot;
            s -= (int)(scroll * HotbarScrollSpeed);

            if (s < 0) s = 11;
            if (s > 11) s = 0;

            Hotbar.SelectedSlot = s;
        }
        Hotbar.HandleHotbarNumberKeys(Hotbar);

        float half = Size / 2;
        Position.X = Math.Clamp(Position.X, half, map.PixelWidth - half);
        Position.Y = Math.Clamp(Position.Y, half, map.PixelHeight - half);
    }
    public void Draw(float zoom)
    {
        bool moving = 
            Raylib.IsKeyDown(KeyboardKey.W) ||
            Raylib.IsKeyDown(KeyboardKey.A) ||
            Raylib.IsKeyDown(KeyboardKey.S) ||
            Raylib.IsKeyDown(KeyboardKey.D);

        Texture2D activeTexture = moving ? WalkTexture : IdleTexture;
        int frameCount = moving ? walkFrames : idleFrames;

        Rectangle src = new Rectangle(
            currentFrame * frameWidth,
            currentDirection * frameHeight,
            frameWidth,
            frameHeight
        );

        Raylib.DrawTextureRec(
            activeTexture,
            src,
            new Vector2(Position.X - frameWidth / 2, Position.Y - frameHeight / 2),
            Color.White
        );
    }

    public void SyncHotbar()
    {
        int hotbarStart = playerInventory.Rows * playerInventory.Columns;

        for (int i = 0; i < playerInventory.Columns; i++)
        {
            Hotbar.Slots[i] = playerInventory.Slots[hotbarStart + i];
        }
    }

    public void DrawHeldItem()
    {
        var stack = Hotbar.Slots[Hotbar.SelectedSlot];
        if (stack == null) return;

        Texture2D tex = stack.Item.Texture;

        float itemScale = 0.75f;
        float itemWidth = tex.Width * itemScale;
        float itemHeight = tex.Height * itemScale;

        Vector2 drawPos = new Vector2(
            Position.X + Size / 2 - itemWidth / 2 - 8,
            Position.Y - itemHeight - 8
        );

        Raylib.DrawTextureEx(
            tex,
            drawPos,
            0f,
            itemScale,
            Color.White
        );
    }
}