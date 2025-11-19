namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Camera
{
    public Vector2 Position;
    public Vector2 Offset;
    public float Zoom;
    public Vector2 CameraVelocity = Vector2.Zero;

    public Camera(Vector2 offset, float initialZoom)
    {
        Offset = offset;
        Zoom = initialZoom;
        Position = Vector2.Zero;
    }

    public void Follow(Player player)
    {
        // Follow only X/Y for a top-down view
        Vector2 target = player.Position - Position;

        float stiffness = 50f;
        float damping = 0.85f;
        float dt = Raylib.GetFrameTime();

        CameraVelocity += target * stiffness * dt;
        CameraVelocity *= damping;

        Position += CameraVelocity * dt;
    }

    public void Begin()
    {
        float grid = 1f / Zoom;

        float snappedX = MathF.Round(Position.X / grid) * grid;
        float snappedY = MathF.Round(Position.Y / grid) * grid;

        Raylib.BeginMode2D(new Camera2D
        {
            Target = new Vector2(snappedX, snappedY),
            Offset = Offset,
            Zoom = Zoom,
            Rotation = 0
        });
    }

    public void End()
    {
        Raylib.EndMode2D();
    }

    public void HandleZoomInput(float dt)
    {
        if (Raylib.IsKeyDown(KeyboardKey.Minus) && (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))) { Zoom -= 0.025f; }
        if (Raylib.IsKeyDown(KeyboardKey.Equal) && (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))) { Zoom += 0.025f; }
        Zoom = Utils.Clamp(Zoom, 0.5f, 3.0f);
    }

    public Camera2D ToRaylibCam()
    {
        return new Camera2D
        {
            Target = Position,
            Offset = Offset,
            Zoom = Zoom,
            Rotation = 0
        };
    }
}
