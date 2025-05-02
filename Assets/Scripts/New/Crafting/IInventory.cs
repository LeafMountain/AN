using System.Collections.Generic;

public interface IInventory
{
    int GetItemCount(string itemId);
    void AddItem(string itemId, int amount);
    void RemoveItem(string itemId, int amount);
    bool HasItems(Dictionary<string, int> items);
    List<InventoryItem> GetInventory();
}