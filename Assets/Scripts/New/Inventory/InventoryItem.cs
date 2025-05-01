using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public int quantity;

    public InventoryItem(string itemID, int quantity = 1)
    {
        this.itemID = itemID;
        this.quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity - amount);
    }
}