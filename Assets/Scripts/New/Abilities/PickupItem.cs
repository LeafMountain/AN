using UnityEngine;

public class PickupItem : MonoBehaviour, IPickup
{
    [SerializeField] private ItemData itemData;
    public int Quantity { get; set; } = 1;

    public string ItemID => itemData.itemID;

    public void OnPickedUp()
    {
        Destroy(gameObject);
    }
}