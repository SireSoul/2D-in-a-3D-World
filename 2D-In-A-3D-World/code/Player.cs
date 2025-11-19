namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Player {
    public Vector2 Position;
    public float Z { get; set; } = 0f;

    private Game game;

    public Vector3 Position3D
    {
        get => new Vector3(Position.X, Position.Y, Z);
        set
        {
            Position = new Vector2(value.X, value.Y);
            Z = value.Z;
        }
    }

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
    public float VerticalVelocity = 0f;          // upward/downward movement
    public float Gravity = 450f;                 // how fast player falls
    public float JumpForce = 200f;               // how strong the jump is
    public bool IsGrounded => Z <= 0.01f;        // touching the floor

    public Player(float x, float y, Map gameMap, Game game)
    {
        Position = new Vector2(x, y);
        Z = 0f; // start on ground level
        this.game = game;

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

        // --- 2D movement (X/Y plane). Z stays the same unless you decide otherwise.
        if (Raylib.IsKeyDown(KeyboardKey.W)) { move.Y -= 1; currentDirection = 1; }
        if (Raylib.IsKeyDown(KeyboardKey.S)) { move.Y += 1; currentDirection = 0; }
        if (Raylib.IsKeyDown(KeyboardKey.A)) { move.X -= 1; currentDirection = 3; }
        if (Raylib.IsKeyDown(KeyboardKey.D)) { move.X += 1; currentDirection = 2; }

        moving = move != Vector2.Zero;

        if (moving)
        {
            move = Vector2.Normalize(move);
            Vector2 attempt = Position + move * Speed * dt;

            Rectangle playerBox = new Rectangle(
                attempt.X - Size / 2,
                attempt.Y - Size / 2,
                Size,
                Size
            );

            if (!CollidesWithWorld(playerBox, game))
                Position = attempt;
        }

        // (Optional later: change Z with keys, e.g. Q/E for floors)

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

        // --- Apply gravity ---
        VerticalVelocity -= Gravity * dt;
        Z += VerticalVelocity * dt;

        // Clamp and reset when hitting the floor
        if (Z < 0f)
        {
            Z = 0f;
            VerticalVelocity = 0f;
        }

        // --- Jump input ---
        if (Raylib.IsKeyDown(KeyboardKey.Space) && IsGrounded)
        {
            VerticalVelocity = JumpForce;   // go up!
        }
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

        // NOTE: still rendering in 2D using X/Y. Z can later be used for layering.
        Vector2 drawPos = new Vector2(
            Position.X - frameWidth / 2,
            Position.Y - frameHeight / 2 - Z      // subtract Z to lift the player
        );

        Raylib.DrawTextureRec(activeTexture, src, drawPos, Color.White);
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

    // private void TryMove(float dx, float dy)
    // {
    //     if (dx == 0 && dy == 0) return;

    //     Vector2 newPos = Position + new Vector2(dx, dy);

    //     // Player bounding box
    //     float half = Size / 3f;
    //     Rectangle bb = new Rectangle(
    //         newPos.X - half,
    //         newPos.Y - half,
    //         Size,
    //         Size
    //     );

    //     // Check collision with world tiles
    //     if (!CollidesWithWorld(bb))
    //     {
    //         Position = newPos;
    //     }
    // }

    private bool CollidesWithWorld(Rectangle playerBox, Game game)
    {
        int left   = (int)(playerBox.X / 16) - 1;
        int right  = (int)((playerBox.X + playerBox.Width) / 16) + 1;
        int top    = (int)(playerBox.Y / 16) - 1;
        int bottom = (int)((playerBox.Y + playerBox.Height) / 16) + 1;

        left = Math.Clamp(left, 0, 72);
        right = Math.Clamp(right, 0, 72);
        top = Math.Clamp(top, 0, 54);
        bottom = Math.Clamp(bottom, 0, 54);

        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                Block? b = game.world[x, y];
                if (b == null || !b.IsSolid)
                    continue;

                Rectangle hb = b.GetHitbox();
                if (Raylib.CheckCollisionRecs(playerBox, hb))
                    return true;
            }
        }

        return false;
    }
}
