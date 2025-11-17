using System.Numerics;
using Raylib_cs;

namespace MyGame;

public abstract class Item
{
    public abstract int Id { get; }
    public abstract string Name { get; }
    public abstract string TexturePath { get; }
    public virtual int MaxStack => 64;

    public Texture2D Texture { get; private set; }
    public Vector2 Position;

    public void LoadTexture()
    {
        Texture = Raylib.LoadTexture(TexturePath);
    }

    public virtual void Update(float dt) { }
    public virtual void Draw() { }
    public virtual void OnPlace() { }
    public virtual void OnBreak() { }
}