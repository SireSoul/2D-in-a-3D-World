namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Camera
{
    public Vector2 Position;
    public Vector2 Offset;   // center of the screen
    public float Zoom;

    public Camera(Vector2 offset, float initialZoom)
    {
        Offset = offset;
        Zoom = initialZoom;
        Position = Vector2.Zero;
    }

    public void Follow(Player player)
    {
        Position = Vector2.Lerp(Position, player.Position, 0.15f);
    }

    public void Begin()
    {
        Camera2D cam = new()
        {
            Target = Position,
            Offset = Offset,
            Zoom = Zoom,
            Rotation = 0
        };

        Raylib.BeginMode2D(cam);
    }

    public void End()
    {
        Raylib.EndMode2D();
    }

    public void HandleZoomInput(float dt)
    {
        float wheel = Raylib.GetMouseWheelMove();
        Zoom += wheel * 0.02f;

        Zoom = Utils.Clamp(Zoom, 0.5f, 3.0f);
    }
}