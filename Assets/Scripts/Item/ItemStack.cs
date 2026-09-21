using System;

[Serializable]
public class ItemStack
{
    public ItemInstance ItemInstance;
    public int Count;

    public bool IsEmpty => (ItemInstance == null);

    public ItemStack(ItemInstance itemInstance, int count)
    {
        ItemInstance = itemInstance;
        Count = count;
    }

    public void Clear()
    {
        ItemInstance = null;
        Count = 0;
    }
}
