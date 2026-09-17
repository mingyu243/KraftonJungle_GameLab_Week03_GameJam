using UnityEngine;

[CreateAssetMenu(menuName = "OriginData/ItemData")]
public class ItemData : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;
    public int MaxStack;
}
