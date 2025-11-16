using System.Numerics;
using MyGame;
using Raylib_cs;

public class Block
{
    public BlockProperties Props {get;}
    public Texture2D Texture {get;}
    public Vector2 Position;
    private int currentFrame;
    private float frameTimer;

    public Block(BlockProperties props, Texture2D texture, Vector2 worldPosition)
    {
        Props = props;
        Texture = texture;
        Position = worldPosition;

        currentFrame = 0;
        frameTimer = 0f;
    }

    public virtual void Update(float dt)
    {
        if (!Props.animated) {return;}

        frameTimer += dt;

        float secondsPerFrame = 1.0f / Props.animationSpeed;

        while (frameTimer >= secondsPerFrame)
        {
            frameTimer -= secondsPerFrame;
            currentFrame = (currentFrame + 1) % Props.animationFrameCount;
        }
    }

    public virtual void Draw()
    {
        Rectangle src;

        if (Props.animated)
        {
            src = new Rectangle(currentFrame * Props.tileSize, 0, Props.tileSize, Props.tileSize);
        } else
        {
            src = new Rectangle(0, 0, Props.tileSize, Props.tileSize);
        }

        Raylib.DrawTextureRec(Texture, src, Position, Color.White);
    }

    public virtual void OnPlace()
    {
        
    }

    public virtual void OnBreak()
    {
        
    }
}
