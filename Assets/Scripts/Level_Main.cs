using Unity.Cinemachine;
using UnityEngine;

public enum LevelState
{
    None,
    Battle,
    Shelter,
    Die
}

public class Level_Main : MonoBehaviour
{
    [Header("Cart")]
    [SerializeField] private CinemachineSplineCart splineCart;
    [SerializeField] private float cartMoveSpeed = 5.0f;

    [Header("Shelter")]
    [SerializeField] private CinemachineSplineCart splineShelter;
    [SerializeField] private int minNextShelterMeter = 100;
    [SerializeField] private int maxNextShelterMeter = 100;

    [Header("Runtime")]
    public LevelState LevelState;
    public float CurrentDistance;
    public int NextShelterMeter;

    void Start()
    {
        // 초기화
        CurrentDistance = 0f;
        splineCart.SplinePosition = CurrentDistance;

        GameManager.Instance.PlayerController.Init();

        // 기본 무기 지급
        ItemInstance gun = OriginDataManager.Instance.GetItem("gun").CreateInstance();
        InventoryManager.Instance.PlayerInventory.AddItemStack(new ItemStack(gun, 1));

        ItemInstance bullet = OriginDataManager.Instance.GetItem("bullet").CreateInstance();
        InventoryManager.Instance.PlayerInventory.AddItemStack(new ItemStack(bullet, 5));

        // 시작
        NextShelterMeter = (int)CurrentDistance + Random.Range(minNextShelterMeter, maxNextShelterMeter);
        splineShelter.SplinePosition = NextShelterMeter;
        LevelState = LevelState.Battle;
        UIManager.Instance.OpenScreen(UIScreenType.HUD);
    }

    void FixedUpdate()
    {
        // 플레이어 죽었으면
        if (GameManager.Instance.PlayerController.CurrentHp <= 0)
        {
            if (LevelState != LevelState.Die)
            {
                LevelState = LevelState.Die;

                UIManager.Instance.CloseAllPopups();
                UIManager.Instance.OpenScreen(UIScreenType.End);
            }

            return;
        }

        // 전투 중이면
        if (LevelState == LevelState.Battle)
        {
            CurrentDistance += cartMoveSpeed * Time.fixedDeltaTime;
            splineCart.SplinePosition = CurrentDistance;

            // 쉘터 도착
            if (CurrentDistance >= NextShelterMeter)
            {
                LevelState = LevelState.Shelter;

                // 보급품 랜덤 생성
                InventoryManager.Instance.SupplyBox.Clear();
                InventoryManager.Instance.SupplyBox.AddItemStack(MakeRandomLootReward());
                InventoryManager.Instance.SupplyBox.AddItemStack(MakeRandomLootReward());

                // 쉘터 UI 띄우기
                UIManager.Instance.CloseAllPopups();
                UIManager.Instance.OpenScreen(UIScreenType.Shelter, new ShelterScreenData()
                {
                    OnClickReady = () =>
                    {

                        NextShelterMeter = (int)CurrentDistance + Random.Range(minNextShelterMeter, maxNextShelterMeter);
                        splineShelter.SplinePosition = NextShelterMeter;
                        LevelState = LevelState.Battle;
                        UIManager.Instance.OpenScreen(UIScreenType.HUD);
                    }
                });
            }
        }
    }

    ItemStack MakeRandomLootReward()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            ItemInstance bullet = OriginDataManager.Instance.GetItem("bullet").CreateInstance();
            return new ItemStack(bullet, Random.Range(2, 4));
        }
        else
        {
            ItemInstance bullet = OriginDataManager.Instance.GetItem("bullet").CreateInstance();
            return new ItemStack(bullet, 1);
        }
    }
}
