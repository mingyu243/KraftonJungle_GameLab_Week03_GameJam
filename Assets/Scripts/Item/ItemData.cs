using System.Collections.Generic;
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

        return instance;
    }
}

[CreateAssetMenu(menuName = "OriginData/MainWeaponData")]
public class MainWeaponData : ItemData
{

}

[CreateAssetMenu(menuName = "OriginData/SubWeaponData")]
public class SubWeaponData : ItemData
{
    public int damage;
}

public class ItemInstance
{
    public ItemData ItemData;
}

public enum ItemCateogry
{
    None,
    Gun, // 주 무기
    Ammo, // 탄약
    SubWeapon, // 보조 무기
    Heal // 회복
}
