using UnityEngine;
using System.Collections.Generic;

public class LootMagnetizer : MonoBehaviour {
    public float magnetRange = 5f;
    public float magnetSpeed = 5f;
    public float depositDistance = 1f;
    public LayerMask lootLayer;

    private PlayerInventory playerInventory;
    private List<Collider> magnetizedLoot = new();

    private readonly Collider[] nearbyLoots = new Collider[1024];

    private void Awake() {
        playerInventory = GetComponent<PlayerInventory>();
        Debug.Log(playerInventory);
    }

    void Update() {
        var size = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, nearbyLoots, lootLayer, QueryTriggerInteraction.Collide);
        for (int i = 0; i < size; i++) {
            Transform loot = nearbyLoots[i].transform;
            if (!magnetizedLoot.Contains(nearbyLoots[i]) && nearbyLoots[i].attachedRigidbody != null) {
                magnetizedLoot.Add(nearbyLoots[i]);
                if (nearbyLoots[i].attachedRigidbody) {
                    nearbyLoots[i].attachedRigidbody.isKinematic = true;
                }

                nearbyLoots[i].isTrigger = true;
            }
        }

        for (int i = magnetizedLoot.Count - 1; i >= 0; i--) {
            if (magnetizedLoot[i] == null) {
                magnetizedLoot.RemoveAt(i);
                continue;
            }

            magnetizedLoot[i].transform.position = Vector3.MoveTowards(magnetizedLoot[i].transform.position, transform.position, magnetSpeed * Time.deltaTime);
            if (Vector3.Distance(magnetizedLoot[i].transform.position, transform.position) < depositDistance) {
                Deposit(magnetizedLoot[i].attachedRigidbody.GetComponent<Lootable>());
                magnetizedLoot.RemoveAt(i);
            }
        }
    }

    void Deposit(Lootable loot) {
        if(loot == null) return;
        playerInventory.AddItem(loot.itemData.itemID, loot.amount);
        Destroy(loot.gameObject);
    }
}