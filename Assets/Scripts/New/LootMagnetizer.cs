using UnityEngine;
using System.Collections.Generic;

public class LootMagnetizer : MonoBehaviour {
    public float magnetRange = 5f;
    public float magnetSpeed = 5f;
    public float depositDistance = 1f;
    public LayerMask lootLayer;

    private PlayerInventory playerInventory;
    private List<Transform> magnetizedLoot = new();

    private Collider[] nearbyLoots;

    private void Awake() {
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Update() {
        var size = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, nearbyLoots, lootLayer);
        for (int i = 0; i < size; i++) {
            Transform loot = nearbyLoots[i].transform;
            if (!magnetizedLoot.Contains(loot))
                magnetizedLoot.Add(loot);
        }

        Debug.Log(size);

        for (int i = magnetizedLoot.Count - 1; i >= 0; i--) {
            Transform loot = magnetizedLoot[i];
            if (loot == null) {
                magnetizedLoot.RemoveAt(i);
                continue;
            }

            loot.position = Vector3.MoveTowards(loot.position, transform.position, magnetSpeed * Time.deltaTime);
            if (Vector3.Distance(loot.position, transform.position) < depositDistance) {
                Deposit(loot.GetComponent<Lootable>());
                magnetizedLoot.RemoveAt(i);
            }
        }
    }

    void Deposit(Lootable loot) {
        playerInventory.AddItem(loot.itemData.itemID, loot.amount);
        Destroy(loot.gameObject);
    }
}