using System.Collections.Generic;
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
    [SerializeField] private float cartMoveSpeed = 3.0f;

    [Header("Shelter")]
    [SerializeField] private CinemachineSplineCart splineShelter;
    [SerializeField] private int minNextShelterMeter = 100;
    [SerializeField] private int maxNextShelterMeter = 100;

    [Header("Enemy1")]
    [SerializeField] private GameObject enemy1Prefab;
    [SerializeField] private Transform[] enemy1SpawnPoints;

    [Header("Enemy2")]
    [SerializeField] private GameObject[] enemy2Prefabs;
    [SerializeField] private Transform enemy2SpawnPointMin;
    [SerializeField] private Transform enemy2SpawnPointMax;

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
        InventoryManager.Instance.PlayerInventory.AddItemStack(new ItemStack(bullet, 3));

        // 시작
        NextShelterMeter = (int)CurrentDistance + Random.Range(minNextShelterMeter, maxNextShelterMeter);
        splineShelter.SplinePosition = NextShelterMeter;
        LevelState = LevelState.Battle;
        UIManager.Instance.OpenScreen(UIScreenType.HUD);

        // 몬스터 생성
        for (int i = 0; i < enemy1SpawnPoints.Length; i++)
        {
            GameObject go = Instantiate(enemy1Prefab);
            go.transform.position = enemy1SpawnPoints[i].transform.position;
        }
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
                List<ItemStack> supplies = MakeRandomSupplies();

                // 첫 보상
                if (NextShelterMeter == 50)
                {
                    // 랜덤 박스로 목표 제시?
                    ItemInstance randomBox = OriginDataManager.Instance.GetItem("random_box").CreateHardCodingInstance();
                    supplies.Add(new ItemStack(randomBox, 1));
                }
                for (int i = 0; i < supplies.Count; i++)
                {
                    InventoryManager.Instance.SupplyBox.AddItemStack(supplies[i]);
                }

                // 아이템에 적용
                List<ItemStack> itemStacks = InventoryManager.Instance.PlayerInventory.ItemStacks;
                for (int i = itemStacks.Count - 1; i >= 0; i--)
                {
                    if (itemStacks[i].ItemInstance.ItemData.Category == ItemCateogry.RandomBox)
                    {
                        // 방금 간 거리만큼 적용
                        itemStacks[i].ItemInstance.CurrentValue += 50;

                        // 거리가 다 도달했다면
                        if (itemStacks[i].ItemInstance.CurrentValue >= itemStacks[i].ItemInstance.GoalValue)
                        {
                            // 보상 다 주고
                            foreach (var item in itemStacks[i].ItemInstance.RewardList)
                            {
                                ItemInstance temp = OriginDataManager.Instance.GetItem(item.itemId).CreateInstance();
                                InventoryManager.Instance.PlayerStorage.AddItemStack(new ItemStack(temp, item.count));
                            }

                            // 자기 삭제
                            InventoryManager.Instance.PlayerInventory.UseItem(itemStacks[i].ItemInstance.Id);
                        }
                    }
                }

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

                // enemy1 생성
                for (int i = 0; i < enemy1SpawnPoints.Length; i++)
                {
                    GameObject go = Instantiate(enemy1Prefab);
                    go.transform.position = enemy1SpawnPoints[i].transform.position;
                }

                // 이전 enemy2 다 제거
                Enemy2Controller[] enemies = FindObjectsByType<Enemy2Controller>(FindObjectsSortMode.None);
                for (int i = 0; i < enemies.Length; i++)
                {
                    Destroy(enemies[i].gameObject);
                }

                // enemy2 생성
                int spawnCount = (int)(CurrentDistance / 50) + 1;
                
                // 위치 저장
                List<Vector3> positions = new();
                if (spawnCount == 1)
                {
                    positions.Add(Vector3.Lerp(enemy2SpawnPointMin.position, enemy2SpawnPointMax.position, 0.5f));
                }
                else
                {
                    for (int i = 1; i <= spawnCount; i++)
                    {
                        float t = (float)i / (spawnCount + 1);
                        Vector3 spawnPos = Vector3.Lerp(enemy2SpawnPointMin.position, enemy2SpawnPointMax.position, t);
                        positions.Add(spawnPos);
                    }
                }

                // 프리팹 생성
                for (int i = 0; i < spawnCount; i++)
                {
                    GameObject randomPrefab = enemy2Prefabs[Random.Range(0, enemy2Prefabs.Length)];

                    GameObject go = Instantiate(randomPrefab);
                    go.transform.position = positions[i];
                }
            }
        }
    }

    List<ItemStack> MakeRandomSupplies()
    {
        List<ItemStack> supllies = new();

        supllies.Add(new ItemStack(OriginDataManager.Instance.GetItem("bullet").CreateInstance(), Random.Range(1, 3)));

        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            supllies.Add(new ItemStack(OriginDataManager.Instance.GetItem("green_herb").CreateInstance(), 1));
        }

        int rand2 = Random.Range(0, 2);
        if (rand2 == 0)
        {
            supllies.Add(new ItemStack(OriginDataManager.Instance.GetItem("random_box").CreateInstance(), 1));
        }

        return supllies;
    }
}
