using System.Numerics;
using Raylib_cs;

namespace MyGame;

public abstract class Block
{
    public abstract int Id { get; }
    public abstract string Name { get; }
    public abstract string TexturePath { get; }
    public virtual bool IsSolid => true;
    public virtual bool Animated => false;
    public int TotalFrames => AnimateHorizontally ? FramesWide : FramesHigh;
    public virtual float AnimationSpeed => 0f;
    public virtual int TileSize => 16;
    public int Rotation { get; set; } = 0;
    public virtual int[] RotationOrder => Enumerable.Range(0, FramesWide).ToArray();
    protected int rotationIndex = 0;

    public Texture2D Texture { get; set; }
    public Vector2 Position;
    public int Z { get; set; } = 0;

    public Vector3 Position3D
    {
        get => new Vector3(Position.X, Position.Y, Z);
        set
        {
            Position = new Vector2(value.X, value.Y);
            Z = (int)value.Z;
        }
    }

    private int currentFrame;
    private float frameTimer;

    public virtual bool AnimateHorizontally => false;  // false = vertical animation
    public virtual int FramesWide => 1;      // columns
    public virtual int FramesHigh => 1;      // rows
    public virtual int AnimationRow => 0;    // which row to animate horizontally in
    public virtual int AnimationColumn => 0; // which column to animate vertically in
    public virtual bool Rotatable => false;

    public void LoadTexture()
    {
        Texture = Raylib.LoadTexture(TexturePath);
    }

    public virtual void Update(float dt)
    {
        if (!Animated) return;

        frameTimer += dt;

        float secondsPerFrame = 1f / AnimationSpeed;

        while (frameTimer >= secondsPerFrame)
        {
            frameTimer -= secondsPerFrame;
            currentFrame = (currentFrame + 1) % TotalFrames;
        }
    }

    public void Draw()
    {
        Rectangle src;

        if (Animated)
        {
            if (AnimateHorizontally)
            {
                // horizontal animation
                src = new Rectangle(
                    currentFrame * TileSize,
                    GetRotationFrame() * TileSize,
                    TileSize,
                    TileSize
                );
            }
            else
            {
                // vertical animation with rotation controlling column
                src = new Rectangle(
                    GetRotationFrame() * TileSize,
                    currentFrame * TileSize,
                    TileSize,
                    TileSize
                );
            }
        }
        else
        {
            // static block, still rotatable
            src = new Rectangle(
                GetRotationFrame() * TileSize,
                0,
                TileSize,
                TileSize
            );
        }

        // For now, we ignore Z when drawing (pure top-down).
        // Later, you can offset by Z or sort draw order by Z.
        Raylib.DrawTextureRec(Texture, src, Position, Color.White);
    }

    public virtual void Rotate()
    {
        if (!Rotatable || RotationOrder.Length == 0) return;

        int nextIndex = (rotationIndex + 1) % RotationOrder.Length;
        SetRotationIndex(nextIndex);
    }

    public void SetRotationIndex(int index)
    {
        if (!Rotatable || RotationOrder.Length == 0)
        {
            rotationIndex = 0;
            Rotation = 0;
            return;
        }

        if (index < 0) index = 0;
        if (index >= RotationOrder.Length) index = RotationOrder.Length - 1;

        rotationIndex = index;
        Rotation = RotationOrder[rotationIndex];
    }

    public virtual int GetRotationFrame()
    {
        return RotationOrder[rotationIndex];
    }


    public virtual void OnPlace() { }
    public virtual void OnBreak() { }
}
