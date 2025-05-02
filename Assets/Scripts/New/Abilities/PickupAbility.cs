using UnityEngine;

public class PickupAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask pickupMask;

    private PlayerInventory inventory;
    public bool IsActive { get; private set; }

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void Activate()
    {
        IsActive = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupMask);
        foreach (var hit in hits)
        {
            var pickup = hit.GetComponent<IPickup>();
            if (pickup != null)
            {
                inventory.AddItem(pickup.ItemID);
                pickup.OnPickedUp();
                break; // Pick up one at a time
            }
        }

        IsActive = false;
    }

    public void Deactivate() { }
    public void Tick() { }
}