using System;
using System.Collections.Generic;

[Serializable]
public class LootRewardSystem
{
    public List<ItemSlot> Slots = new();

    public bool AddItem(ItemStack data, int count)
    {
        return true;
    }

    public bool RemoveItem(ItemData data, int count)
    {
        return true;
    }
}
