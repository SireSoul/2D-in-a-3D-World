using System.Numerics;
using Raylib_cs;

namespace MyGame;

public class ShaftBlock : Block
{
    public override int Id => 1;
    public override string Name => "Shaft";
    public override string TexturePath => "assets/blocks/textures/shaft.png";

    // New sheet: 4 columns × 4 rows
    public override int FramesWide  => 4;  // 4 rotation variants across
    public override int FramesHigh  => 4;  // 4 animation frames down
    public override int TileSize    => 16;

    // All rotations should animate the same way (VERTICALLY)
    public override bool Animated => true;
    public override float AnimationSpeed => 6f;

    // This block can rotate; rotation = which column we use (0–3)
    public override bool Rotatable => true;

    // Rotation cycle: just walk through columns 0..3
    public override int[] RotationOrder => new int[]
    {
        0,
        1,
        2,
        3,
        4,
        5
    };

    // IMPORTANT: always vertical animation now
    public override bool AnimateHorizontally => false;

    // For vertical animation, X = column (rotation), Y = frame
    public override int GetRotationFrame()
    {
        // Base Block.Rotate() keeps rotationIndex in range of RotationOrder
        return RotationOrder[rotationIndex];
    }

    public override void OnPlace()
    {
        Console.WriteLine("Placed a Shaft!");
    }

    public override void Update(float dt)
    {
        base.Update(dt); // animation logic
    }
}
