using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OriginDataManager : MonoBehaviour
{
    public static OriginDataManager Instance;

    [SerializeField] private List<ItemData> itemDataList;

    private Dictionary<string, ItemData> itemDict;

    private void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        itemDict = itemDataList.ToDictionary(x => x.Id, x => x);
    }

    public ItemData GetItem(string id)
    {
        return itemDict.GetValueOrDefault(id);
    }
}
