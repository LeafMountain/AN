using UnityEngine;

public class ResourceOnDamage : MonoBehaviour {
    [SerializeField] private ItemData itemData;
    [SerializeField] private float amountPerDamage = 0.25f;
    [SerializeField] private int onDestroyBonus = 5;
    [SerializeField] private bool dontRequirePickup;

    private HealthComponent health;
    private Lootable lootablePrefab => GameManager.Instance.lootablePrefab;

    private void Awake() {
        health = GetComponent<HealthComponent>();
        if (health == null) return;

        health.OnDamaged.AddListener(HandleDamage);
        health.OnDeath.AddListener(HandleDestroyed);
    }

    private void OnDestroy() {
        if (health == null) return;

        health.OnDamaged.RemoveListener(HandleDamage);
        health.OnDeath.RemoveListener(HandleDestroyed);
    }

    private void HandleDamage(float damage) {
        int amount = Mathf.FloorToInt(damage * amountPerDamage);
        if (amount > 0) {
            TryGiveToLastAttacker(amount); // See note below
        }
    }

    private void HandleDestroyed() {
        TryGiveToLastAttacker(onDestroyBonus);
    }

    // Replace this with however you track the attacker
    private void TryGiveToLastAttacker(int amount) {
        if (dontRequirePickup) {
            var player = health.lastAttacker;
            if (player != null && player.TryGetComponent<IInventory>(out var provider)) {
                provider.AddItem(itemData.itemID, amount);
            }
        }
        else {
            SpawnDrops(amount); 
        }
    }

    private void SpawnDrops(int amount) {
        for (int i = 0; i < amount; i++) {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.5f;
            spawnPos.y = transform.position.y + 1f; // ensure above ground

            Lootable pickup = Instantiate(lootablePrefab, spawnPos, Quaternion.identity);

            if (pickup.TryGetComponent<Rigidbody>(out var rb)) {
                rb.AddForce(Random.onUnitSphere * 2f, ForceMode.Impulse);
            }

            var lootable = pickup.GetComponent<Lootable>();
            if (lootable != null) {
                lootable.Initialize(itemData, 1); // spawn one unit per drop
                // pickupComponent.Quantity = amount;
            }
        }
    }
}