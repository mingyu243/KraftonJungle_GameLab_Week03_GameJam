using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Inventory SupplyBox = new();
    public Inventory PlayerInventory = new();
    public Inventory PlayerStorage = new();

    [Header("Settings")]
    [SerializeField] private int supplyBoxBaseCapacity = 5;
    [SerializeField] private int inventoryBaseCapacity = 3;
    [SerializeField] private int storageBaseCapacity = 40;

    public event Action OnInventoryChanged;

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void NotifyInventoryChange()
    {
        OnInventoryChanged?.Invoke();
    }

    void OnEnable()
    {
        UniTask.Void(async () =>
        {
            await UniTask.WaitUntil(() => InputManager.Instance != null);
            InputManager.Instance.OnInventory += OnInventory;
        });
    }

    void OnDisable()
    {
        InputManager.Instance.OnInventory -= OnInventory;
    }

    void Start()
    {
        SupplyBox.Capacity = supplyBoxBaseCapacity;
        PlayerInventory.Capacity = inventoryBaseCapacity;
        PlayerStorage.Capacity = storageBaseCapacity;
    }

    void OnInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            // 배틀 중에만 조작 가능
            if (GameManager.Instance.Level_Main.LevelState == LevelState.Battle)
            {
                // 인벤토리가 켜져있으면 끄기
                if (UIManager.Instance.IsPopupOpen(UIPopupType.Inventory))
                {
                    Time.timeScale = 1f;
                    UIManager.Instance.ClosePopup(UIPopupType.Inventory);
                }
                else
                {
                    Time.timeScale = 0f;
                    UIManager.Instance.OpenPopup(UIPopupType.Inventory);
                }
            }
        }
    }
}
