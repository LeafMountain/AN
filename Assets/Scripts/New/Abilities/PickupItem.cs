using UnityEngine;

public class PickupItem : MonoBehaviour, IPickup
{
    [SerializeField] private ItemData itemData;

    public string ItemID => itemData.itemID;

    public void OnPickedUp()
    {
        Destroy(gameObject);
    }
}