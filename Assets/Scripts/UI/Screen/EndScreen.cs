using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour, IUIScreen
{
    [SerializeField] private TMP_Text currentDistanceText;
    [SerializeField] private ItemSlotUI slotUIPrefab;

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

        tooltip.SetActive(false);

        currentDistanceText.text = $"기록 : {GameManager.Instance.Level_Main.CurrentDistance.ToString("F0")}m";

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
        InventoryManager.Instance.OnInventoryChanged -= OnInventoryChanged;
    }

    public void OnClickRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnInventoryChanged()
    {
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
        }
    }
}
