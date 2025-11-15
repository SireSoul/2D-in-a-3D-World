namespace MyGame;

using Raylib_cs;
using System.Numerics;

public class Player {
    public Vector2 Position;
    Vector2 move = Vector2.Zero;

    public float Speed = 120f;
    public float Size = 16f;
    public Player(float x, float y)
    {
        Position = new Vector2(x, y);
    }

    public void Update(float dt)
    {
        if (Raylib.IsKeyDown(KeyboardKey.W)) {move.Y -= 1;}
        if (Raylib.IsKeyDown(KeyboardKey.S)) {move.Y += 1;}
        if (Raylib.IsKeyDown(KeyboardKey.D)) {move.X += 1;}
        if (Raylib.IsKeyDown(KeyboardKey.A)) {move.X -= 1;}

        if (move.LengthSquared() > 1f)
        {
            move = Vector2.Normalize(move);
        }
        Position += move * Speed * dt;
    }
    public void Draw()
    {
        Raylib.DrawRectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size,
            (int)Size,
            Color.White
        );
    }
}