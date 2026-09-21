using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "OriginData/ItemData")]
public class ItemData : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite Icon;
    public string Description;
    
    public ItemCateogry Category;

    public ItemInstance CreateInstance()
    {
        ItemInstance instance = new ItemInstance()
        {
            ItemData = this,
        };

        if (Category == ItemCateogry.RandomBox)
        {
            instance.Id = Guid.NewGuid().ToString();

            int rand = UnityEngine.Random.Range(0, 10);
            if (rand >= 6) // 6, 7, 8, 9
            {
                instance.GoalValue = 100;

                int rand2 = UnityEngine.Random.Range(0, 2);
                if (rand2 == 0)
                {
                    instance.RewardList.Add(("bullet", 5));
                    instance.RewardList.Add(("red_herb", 1));
                }
                else if (rand2 == 1)
                {
                    instance.RewardList.Add(("bullet", 3));
                    instance.RewardList.Add(("red_herb", 1));
                }
            }
            else if (rand >= 3) // 3, 4, 5
            {
                instance.GoalValue = 200;

                int rand2 = UnityEngine.Random.Range(0, 2);
                if (rand2 == 0)
                {
                    instance.RewardList.Add(("bullet", 20));
                    instance.RewardList.Add(("red_herb", 1));
                }
                else if (rand2 == 1)
                {
                    instance.RewardList.Add(("bullet", 10));
                    instance.RewardList.Add(("red_herb", 1));
                    instance.RewardList.Add(("red_herb", 1));
                }
            }
            else // 0, 1, 2
            {
                instance.GoalValue = 50;

                int rand2 = UnityEngine.Random.Range(0, 2);
                if (rand2 == 0)
                {
                    instance.RewardList.Add(("bullet", 3));
                }
                else if (rand2 == 1)
                {
                    instance.RewardList.Add(("green_herb", 1));
                }
            }
        }
        else
        {
            instance.Id = Id;
        }

        return instance;
    }

    public ItemInstance CreateHardCodingInstance()
    {
        ItemInstance instance = new ItemInstance()
        {
            ItemData = this,
        };

        instance.Id = Guid.NewGuid().ToString();
        instance.GoalValue = 500;

        instance.RewardList.Add(("bullet", 999));
        instance.RewardList.Add(("red_herb", 1));
        instance.RewardList.Add(("red_herb", 1));
        instance.RewardList.Add(("star", 1));
        instance.RewardList.Add(("star", 1));
        instance.RewardList.Add(("star", 1));
        instance.RewardList.Add(("star", 1));

        return instance;
    }
}

public class ItemInstance
{
    public string Id;

    public ItemData ItemData;

    public string Description 
    {
        get
        {
            if (ItemData.Category == ItemCateogry.RandomBox)
            {
                int remainingDistance = Mathf.Max(0, GoalValue - CurrentValue);
                return string.Format(ItemData.Description, GoalValue, remainingDistance);
            }
            else
            {
                return ItemData.Description;
            }
        }
     
    }

    public int GoalValue;
    public int CurrentValue = 0;

    public List<(string itemId, int count)> RewardList = new();
}

public enum ItemCateogry
{
    None,
    Gun, // 주 무기
    Ammo, // 탄약
    SubWeapon, // 보조 무기
    Heal, // 회복
    RandomBox
}
