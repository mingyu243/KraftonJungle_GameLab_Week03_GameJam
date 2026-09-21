using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    public ItemUI ItemUI;

    public void SetItemStack(ItemStack itemStack)
    {
        ItemUI.IconImage.gameObject.SetActive(true);
        ItemUI.IconImage.sprite = itemStack.ItemInstance.ItemData.Icon;

        if (itemStack.ItemInstance.ItemData.Category == ItemCateogry.Ammo)
        {
            ItemUI.CountText.text = $"x{itemStack.Count}";
        }
        else
        {
            ItemUI.CountText.text = string.Empty;
        }
    }

    public void Clear()
    {
        ItemUI.OnPointerEnterEvent = null;
        ItemUI.OnPointerExitEvent = null;
        ItemUI.OnPointerClickEvent = null;

        ItemUI.IconImage.gameObject.SetActive(false);
        ItemUI.CountText.text = string.Empty;
    }
}
