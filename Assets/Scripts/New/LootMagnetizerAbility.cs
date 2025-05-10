using System;
using System.Collections.Generic;
using UnityEngine;

public class LootMagnetizerAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private float magnetSpeed = 5f;
    [SerializeField] private float depositDistance = 1f;
    [SerializeField] private LayerMask lootLayer;

    private PlayerInventory playerInventory;
    private List<Collider> magnetizedLoot = new();
    private readonly Collider[] nearbyLoots = new Collider[1024];

    public bool IsActive { get; private set; }

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        magnetizedLoot.Clear(); // Clear magnetized loot when deactivated
    }

    public void Tick()
    {
        if (!IsActive) return;

        var size = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, nearbyLoots, lootLayer, QueryTriggerInteraction.Collide);
        for (int i = 0; i < size; i++)
        {
            if (!magnetizedLoot.Contains(nearbyLoots[i]) && nearbyLoots[i].attachedRigidbody != null)
            {
                magnetizedLoot.Add(nearbyLoots[i]);
                if (nearbyLoots[i].attachedRigidbody)
                {
                    nearbyLoots[i].attachedRigidbody.isKinematic = true;
                }

                nearbyLoots[i].isTrigger = true;
            }
        }

        for (int i = magnetizedLoot.Count - 1; i >= 0; i--)
        {
            if (magnetizedLoot[i] == null)
            {
                magnetizedLoot.RemoveAt(i);
                continue;
            }

            magnetizedLoot[i].transform.position = Vector3.MoveTowards(magnetizedLoot[i].transform.position, transform.position, magnetSpeed * Time.deltaTime);
            if (Vector3.Distance(magnetizedLoot[i].transform.position, transform.position) < depositDistance)
            {
                Deposit(magnetizedLoot[i].attachedRigidbody.GetComponent<Lootable>());
                magnetizedLoot.RemoveAt(i);
            }
        }
    }

    private void Deposit(Lootable loot)
    {
        if (loot == null) return;
        playerInventory.AddItem(loot.itemData.itemID, loot.amount);
        Destroy(loot.gameObject);
    }
}