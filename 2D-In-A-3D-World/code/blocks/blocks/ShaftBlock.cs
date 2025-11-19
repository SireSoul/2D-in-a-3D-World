using System.Numerics;
using Raylib_cs;

namespace MyGame;

public class ShaftBlock : Block
{
    public override int Id => 1;
    public override string Name => "Shaft";
    public override string TexturePath => "assets/blocks/textures/shaft.png";
    public override int FramesWide => 4;
    public override int FramesHigh => 4;
    public override int TileSize => 16;
    public override bool Animated => true;
    public override float AnimationSpeed => 6f;
    public override bool Rotatable => true;
    public override int[] RotationOrder => new int[]
    {
        0,
        1,
        2,
        3,
        4,
        5
    };
    public override bool AnimateHorizontally => false;
    public override int MaxHardness => 1;
    public override int GetRotationFrame()
    {
        return RotationOrder[rotationIndex];
    }

    public override void OnPlace()
    {
        Console.WriteLine("Placed a Shaft!");
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }
    public override Rectangle GetHitbox()
    {
        return new Rectangle(
            Position.X + 6.5f,
            Position.Y + 5.5f,
            2,
            2
        );
    }
}
