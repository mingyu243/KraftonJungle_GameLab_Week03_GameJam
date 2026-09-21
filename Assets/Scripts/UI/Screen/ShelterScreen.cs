using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShelterScreenData
{
    public UnityAction OnClickReady;
}

public class ShelterScreen : MonoBehaviour, IUIScreen
{
    [SerializeField] private Button readyButton;

    [SerializeField] private ItemSlotUI slotUIPrefab;

    [Header("Supply Box")]
    [SerializeField] private Transform supplyBoxSlotParentTr;
    private List<ItemSlotUI> supplyBoxItemSlotUIList = new();

    [Header("Inventory")]
    [SerializeField] private Transform inventorySlotParentTr;
    private List<ItemSlotUI> inventoryItemSlotUIList = new();

    [Header("Storage")]
    [SerializeField] private Transform storageSlotParentTr;
    private List<ItemSlotUI> storageItemSlotUIList = new();

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private TMP_Text tooltipTitleText;
    [SerializeField] private TMP_Text tooltipDescriptionText;

    public void Open(object data = null)
    {
        Time.timeScale = 0f;
        GameManager.Instance.SetVisibleCursor(true);

        if (data is ShelterScreenData screenData)
        {
            readyButton.onClick.AddListener(screenData.OnClickReady);
        }

        tooltip.SetActive(false);

        // 보급품

        // 기존 슬롯 다 삭제
        for (int i = (supplyBoxItemSlotUIList.Count - 1); i >= 0; i--)
        {
            Destroy(supplyBoxItemSlotUIList[i].gameObject);
            supplyBoxItemSlotUIList.RemoveAt(i);
        }
        // 다시 슬롯 생성
        for (int i = 0; i < InventoryManager.Instance.SupplyBox.Capacity; i++)
        {
            ItemSlotUI slotUI = Instantiate(slotUIPrefab);
            slotUI.transform.SetParent(supplyBoxSlotParentTr);
            slotUI.transform.localScale = Vector3.one;
            slotUI.Clear();

            supplyBoxItemSlotUIList.Add(slotUI);
        }

        // 인벤토리

        // 기존 슬롯 다 삭제
        for (int i = (inventoryItemSlotUIList.Count - 1); i >= 0; i--)
        {
            Destroy(inventoryItemSlotUIList[i].gameObject);
            inventoryItemSlotUIList.RemoveAt(i);
        }
        // 다시 슬롯 생성
        for (int i = 0; i < InventoryManager.Instance.PlayerInventory.Capacity; i++)
        {
            ItemSlotUI slotUI = Instantiate(slotUIPrefab);
            slotUI.transform.SetParent(inventorySlotParentTr);
            slotUI.transform.localScale = Vector3.one;
            slotUI.Clear();

            inventoryItemSlotUIList.Add(slotUI);
        }

        // 보관함

        // 기존 슬롯 다 삭제
        for (int i = (storageItemSlotUIList.Count - 1); i >= 0; i--)
        {
            Destroy(storageItemSlotUIList[i].gameObject);
            storageItemSlotUIList.RemoveAt(i);
        }
        // 다시 슬롯 생성
        for (int i = 0; i < InventoryManager.Instance.PlayerStorage.Capacity; i++)
        {
            ItemSlotUI slotUI = Instantiate(slotUIPrefab);
            slotUI.transform.SetParent(storageSlotParentTr);
            slotUI.transform.localScale = Vector3.one;
            slotUI.Clear();

            storageItemSlotUIList.Add(slotUI);
        }

        InventoryManager.Instance.OnInventoryChanged += OnInventoryChanged;
        OnInventoryChanged();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SetVisibleCursor(false);

        readyButton.onClick.RemoveAllListeners();
    }

    private void OnInventoryChanged()
    {
        // 보급품

        // 슬롯 다 비우고
        for (int i = 0; i < supplyBoxItemSlotUIList.Count; i++)
        {
            supplyBoxItemSlotUIList[i].Clear();
        }
        // 슬롯에 아이템 보여주기
        for (int i = 0; i < InventoryManager.Instance.SupplyBox.ItemStacks.Count; i++)
        {
            ItemStack targetItemStack = InventoryManager.Instance.SupplyBox.ItemStacks[i];
            supplyBoxItemSlotUIList[i].SetItemStack(targetItemStack);

            // 아이템 반응
            ItemUI itemUI = supplyBoxItemSlotUIList[i].ItemUI;

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
        }

        // 인벤토리

        // 슬롯 다 비우고
        for (int i = 0; i < inventoryItemSlotUIList.Count; i++)
        {
            inventoryItemSlotUIList[i].Clear();
        }
        // 슬롯에 아이템 보여주기
        for (int i = 0; i < InventoryManager.Instance.PlayerInventory.ItemStacks.Count; i++)
        {
            ItemStack targetItemStack = InventoryManager.Instance.PlayerInventory.ItemStacks[i];
            inventoryItemSlotUIList[i].SetItemStack(targetItemStack);

            // 아이템 반응
            ItemUI itemUI = inventoryItemSlotUIList[i].ItemUI;

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
                tooltip.SetActive(false);

                UIManager.Instance.OpenPopup(UIPopupType.Confirm, new ComfirmPopupData()
                {
                    Title = targetItemStack.ItemInstance.ItemData.Name,

                    LeftButtonText = "보관",
                    OnClickLeftButton = () =>
                    {
                        // 아이템 옮기기
                        InventoryManager.Instance.PlayerStorage.AddItemStack(targetItemStack);
                        InventoryManager.Instance.PlayerInventory.RemoveItemStack(targetItemStack);

                        UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                    },
                    RightButtonText = "취소",
                    OnClickRightButton = () =>
                    {
                        UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                    }
                });
            };
        }

        // 보관함

        // 슬롯 다 비우고
        for (int i = 0; i < storageItemSlotUIList.Count; i++)
        {
            storageItemSlotUIList[i].Clear();
        }
        // 슬롯에 아이템 보여주기
        for (int i = 0; i < InventoryManager.Instance.PlayerStorage.ItemStacks.Count; i++)
        {
            ItemStack targetItemStack = InventoryManager.Instance.PlayerStorage.ItemStacks[i];
            storageItemSlotUIList[i].SetItemStack(targetItemStack);

            // 아이템 반응
            ItemUI itemUI = storageItemSlotUIList[i].ItemUI;

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
                tooltip.SetActive(false);

                UIManager.Instance.OpenPopup(UIPopupType.Confirm, new ComfirmPopupData()
                {
                    Title = targetItemStack.ItemInstance.ItemData.Name,

                    LeftButtonText = "꺼내기",
                    OnClickLeftButton = () =>
                    {
                        // 아이템 옮기기
                        InventoryManager.Instance.PlayerInventory.AddItemStack(targetItemStack);
                        InventoryManager.Instance.PlayerStorage.RemoveItemStack(targetItemStack);

                        UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                    },
                    RightButtonText = "취소",
                    OnClickRightButton = () =>
                    {
                        UIManager.Instance.ClosePopup(UIPopupType.Confirm);
                    }
                });
            };
        }
    }

    public void OnClickGetSupplyBoxButton()
    {
        // 보급품에 있는 아이템
        for (int i = 0; i < InventoryManager.Instance.SupplyBox.ItemStacks.Count; i++)
        {
            ItemStack targetItemStack = InventoryManager.Instance.SupplyBox.ItemStacks[i];

            // 보관함으로 옮겨주기
            InventoryManager.Instance.PlayerStorage.AddItemStack(targetItemStack);
        }

        InventoryManager.Instance.SupplyBox.Clear();
    }
}
