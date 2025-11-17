namespace MyGame;

public class Stone : Item
{
    public override int Id => 1;
    public override string Name => "Stone";
    public override string TexturePath => "assets/items/textures/stone.png";
    public override int MaxStack => 999;
}
