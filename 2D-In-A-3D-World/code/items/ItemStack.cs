namespace MyGame;

public class ItemStack
{
    public Item Item { get; }
    public int Count { get; private set; }
    public int MaxStackSize => Item.MaxStack;

    public ItemStack(Item item, int count = 1)
    {
        Item = item;
        Count = Math.Clamp(count, 0, MaxStackSize);
    }

    public bool IsFull()
    {
        return Count >= MaxStackSize;
    }

    public bool CanMerge(Item other)
    {
        return other.Id == Item.Id;
    }

    public void Add(int amount)
    {
        Count = Math.Min(Count + amount, MaxStackSize);
    }

    public void Remove(int amount)
    {
        Count = Math.Max(0, Count - amount);
    }
}
