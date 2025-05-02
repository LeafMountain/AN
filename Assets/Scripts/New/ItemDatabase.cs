using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> items;
    private Dictionary<string, ItemData> lookup;

    public void Initialize()
    {
        lookup = new();
        foreach (var item in items)
            lookup[item.itemID] = item;
    }

    public ItemData Get(string itemID)
    {
        if (lookup == null) Initialize();
        return lookup.TryGetValue(itemID, out var data) ? data : null;
    }
}