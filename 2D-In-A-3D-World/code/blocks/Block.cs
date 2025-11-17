using System.Numerics;
using Raylib_cs;

namespace MyGame;

public abstract class Block
{
    public abstract int Id { get; }
    public abstract string Name { get; }
    public abstract string TexturePath { get; }

    public virtual bool IsSolid => true;

    // Animation
    public virtual bool Animated => false;
    public virtual int FrameCount => 1;
    public virtual float AnimationSpeed => 0f;     // frames per second
    public virtual int TileSize => 16;

    public Texture2D Texture { get; private set; }
    public Vector2 Position;

    private int currentFrame;
    private float frameTimer;

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
            currentFrame = (currentFrame + 1) % FrameCount;
        }
    }

    public virtual void Draw()
    {
        Rectangle src;

        if (Animated)
        {
            src = new Rectangle(currentFrame * TileSize, 0, TileSize, TileSize);
        }
        else
        {
            src = new Rectangle(0, 0, TileSize, TileSize);
        }

        Raylib.DrawTextureRec(Texture, src, Position, Color.White);
    }

    public virtual void OnPlace() { }
    public virtual void OnBreak() { }
}