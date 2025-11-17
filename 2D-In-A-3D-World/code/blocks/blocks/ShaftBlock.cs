using System.Numerics;
using Raylib_cs;

namespace MyGame;

public class ShaftBlock : Block
{
    private float debugTimer;

    public override int Id => 1;  // whatever ID you choose
    public override string Name => "Shaft";
    public override string TexturePath => "assets/blocks/textures/shaft.png";

    // Animation? set these:
    public override bool Animated => true;
    public override int FrameCount => 4;
    public override float AnimationSpeed => 6f;
    public override int TileSize => 16;

    public override void OnPlace()
    {
        Console.WriteLine("Placed a Shaft!");
    }

    public override void Update(float dt)
    {
        debugTimer += dt;
        if (debugTimer >= 1f)
        {
            Console.WriteLine("Shaft ticking...");
            debugTimer = 0f;
        }

        base.Update(dt); // animation logic
    }
}