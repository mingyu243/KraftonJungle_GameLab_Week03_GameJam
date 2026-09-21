using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPopup : MonoBehaviour, IUIPopup
{
    [Header("Health")]
    [SerializeField] private GameObject[] hpObjects;

    [Header("Inventory")]
    [SerializeField] private Transform slotParentTr;
    [SerializeField] private ItemSlotUI slotUIPrefab;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private TMP_Text tooltipTitleText;
    [SerializeField] private TMP_Text tooltipDescriptionText;

    private List<ItemSlotUI> itemSlotUIList = new();

    public void Open(object data = null)
    {
        Time.timeScale = 0f;
        GameManager.Instance.SetVisibleCursor(true);

        tooltip.SetActive(false);

        // 체력
        foreach (var item in hpObjects)
        {
            item.SetActive(false);
        }
        int index = GameManager.Instance.PlayerController.CurrentHp - 1;
        hpObjects[index].SetActive(true);

        // 인벤토리

        // 기존 슬롯 다 삭제
        for (int i = (itemSlotUIList.Count - 1); i >= 0 ; i--)
        {
            Destroy(itemSlotUIList[i].gameObject);
            itemSlotUIList.RemoveAt(i);
        }
        // 다시 슬롯 생성
        int capacity = InventoryManager.Instance.PlayerInventory.Capacity;
        for (int i = 0; i < capacity; i++)
        {
            ItemSlotUI slotUI = Instantiate(slotUIPrefab);
            slotUI.transform.SetParent(slotParentTr);
            slotUI.transform.localScale = Vector3.one;
            slotUI.Clear();

            itemSlotUIList.Add(slotUI);
        }

        InventoryManager.Instance.OnInventoryChanged += OnInventoryChanged;
        OnInventoryChanged();
    }
    public void Close()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SetVisibleCursor(false);

        InventoryManager.Instance.OnInventoryChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged()
    {
        // 인벤토리

        // 슬롯 다 비우고
        for (int i = 0; i < itemSlotUIList.Count; i++)
        {
            itemSlotUIList[i].Clear();
        }
        // 슬롯에 아이템 보여주기
        int count = InventoryManager.Instance.PlayerInventory.ItemStacks.Count;
        for (int i = 0; i < count; i++)
        {
            ItemStack targetItemStack = InventoryManager.Instance.PlayerInventory.ItemStacks[i];
            itemSlotUIList[i].SetItemStack(targetItemStack);

            // 아이템 반응
            ItemUI itemUI = itemSlotUIList[i].ItemUI;

            itemUI.OnPointerEnterEvent += () =>
            {
                tooltip.transform.position = itemUI.transform.position;
                tooltipTitleText.text = targetItemStack.ItemInstance.ItemData.Name;
                tooltipDescriptionText.text = targetItemStack.ItemInstance.ItemData.Description;

                tooltip.SetActive(true);
            };
            itemUI.OnPointerExitEvent += () =>
            {
                tooltip.SetActive(false);
            };
            itemUI.OnPointerClickEvent += () =>
            {
                // 회복 아이템일 때만
                if (targetItemStack.ItemInstance.ItemData.Category == ItemCateogry.Heal)
                {
                    tooltip.SetActive(false);

                    UIManager.Instance.OpenPopup(UIPopupType.Confirm, new ComfirmPopupData()
                    {
                        Title = targetItemStack.ItemInstance.ItemData.Name,

                        LeftButtonText = "사용",
                        OnClickLeftButton = () =>
                        {
                            // 풀피가 아니라면
                            if (GameManager.Instance.PlayerController.CurrentHp < GameManager.Instance.PlayerController.MaxHp)
                            {
                                // 아이템 사용 처리
                                GameManager.Instance.PlayerController.CurrentHp++;
                                InventoryManager.Instance.PlayerInventory.UseItem(targetItemStack.ItemInstance.ItemData.Id);
                                
                                UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                            }
                        },
                        RightButtonText = "취소",
                        OnClickRightButton = () =>
                        {
                            UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                        }
                    });
                }
            };
        }
    }
}
