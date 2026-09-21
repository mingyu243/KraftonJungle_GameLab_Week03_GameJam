using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    public List<ItemStack> ItemStacks = new();

    public int Capacity;

    public void AddItemStack(ItemStack itemStack)
    {
        if (itemStack.ItemInstance.ItemData.Category == ItemCateogry.Ammo)
        {
            for (int i = 0; i < ItemStacks.Count; i++)
            {
                // 이미 있는 아이템이라면
                if (ItemStacks[i].ItemInstance.ItemData.Id == itemStack.ItemInstance.ItemData.Id)
                {
                    ItemStacks[i].Count += itemStack.Count;
                    InventoryManager.Instance.NotifyInventoryChange();
                    return;
                }
            }
        }

        // 없는 아이템이라면
        ItemStacks.Add(itemStack);
        InventoryManager.Instance.NotifyInventoryChange();
    }

    public void RemoveItemStack(ItemStack itemStack)
    {
        ItemStacks.Remove(itemStack);
        InventoryManager.Instance.NotifyInventoryChange();
    }

    public void UseItem(string itemId)
    {
        for (int i = 0; i < ItemStacks.Count; i++)
        {
            // 이미 있는 아이템이라면
            if (ItemStacks[i].ItemInstance.ItemData.Id == itemId)
            {
                ItemStacks[i].Count--;
                if (ItemStacks[i].Count == 0)
                {
                    ItemStacks.RemoveAt(i);
                }
                InventoryManager.Instance.NotifyInventoryChange();

                return;
            }
        }
    }

    public void Clear()
    {
        ItemStacks.Clear();
        InventoryManager.Instance.NotifyInventoryChange();
    }
}
