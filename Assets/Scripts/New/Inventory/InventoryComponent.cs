using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    
    // The inventory stores serialized ItemID and quantity
    private List<InventoryItem> inventory = new List<InventoryItem>();
    
    public static event Action<string, int> OnItemAdded;

    // You might use SyncVars, RPCs, or Commands to sync this over the network later
    public List<InventoryItem> GetInventory() => new List<InventoryItem>(inventory);

    // Add item to inventory (handles stacking)
    public void AddItem(string itemID, int amount = 1)
    {
        var existingItem = inventory.Find(item => item.itemID == itemID);

        if (existingItem != null)
        {
            existingItem.AddQuantity(amount);
        }
        else
        {
            inventory.Add(new InventoryItem(itemID, amount));
        }

        Debug.Log($"Added {amount} of {itemID}.");
        
        OnItemAdded?.Invoke(itemID, amount);
    }

    // Remove item from inventory
    public void RemoveItem(string itemID, int amount = 1)
    {
        var existingItem = inventory.Find(item => item.itemID == itemID);

        if (existingItem != null)
        {
            existingItem.RemoveQuantity(amount);
            if (existingItem.quantity == 0)
                inventory.Remove(existingItem);
        }
        else
        {
            Debug.LogWarning($"Item {itemID} not found in inventory.");
        }

        Debug.Log($"Removed {amount} of {itemID}.");
    }

    // For network syncing, you might want to expose public methods that sync state with the server
    // Example: Networked methods to sync inventory over the network
    public void SyncInventoryWithNetwork()
    {
        // Sync with the server (custom logic with your networking solution)
    }
}