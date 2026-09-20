using System;

[Serializable]
public class ItemSlot
{
    public int SlotIndex;
    public ItemInstance Item;
    public int Count;

    public bool IsEmpty => (Item == null);

    public void Clear()
    {
        Item = null;
        Count = 0;
    }
}
