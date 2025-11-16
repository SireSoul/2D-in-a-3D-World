namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Player {
    public Vector2 Position;
    public Vector2 Velocity;
    Vector2 move = Vector2.Zero;

    public float Speed = 120f;
    public float Size = 16f;
    private Map map;

    public Player(float x, float y, Map gameMap)
    {
        Position = new Vector2(x, y);
        map = gameMap;
    }

    public void Update(float dt)
    {
        Vector2 input = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W)) input.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S)) input.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A)) input.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D)) input.X += 1;

        if (input.LengthSquared() > 0)
            input = Vector2.Normalize(input);

        Vector2 targetVel = input * Speed;

        float accel = 20f;
        float decel = 16f;

        if (input != Vector2.Zero)
            Velocity = Vector2.Lerp(Velocity, targetVel, accel * dt);
        else
            Velocity = Vector2.Lerp(Velocity, Vector2.Zero, decel * dt);

        Position += Velocity * dt;

        float half = Size / 2;
        Position.X = Math.Clamp(Position.X, half, map.PixelWidth - half);
        Position.Y = Math.Clamp(Position.Y, half, map.PixelHeight - half);
    }
    public void Draw(float zoom)
    {
        float grid = 1f / zoom;

        float snappedX = MathF.Round(Position.X / grid) * grid;
        float snappedY = MathF.Round(Position.Y / grid) * grid;

        Raylib.DrawRectangle(
            (int)(snappedX - Size / 2),
            (int)(snappedY - Size / 2),
            (int)Size,
            (int)Size,
            Color.White
        );
    }
}