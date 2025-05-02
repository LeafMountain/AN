using UnityEngine;
using UnityEngine.Serialization;

public class ResourceOnDamage : MonoBehaviour {
    [SerializeField] private ItemData itemData;
    [SerializeField] private float amountPerDamage = 0.25f;
    [SerializeField] private int onDestroyBonus = 5;

    private HealthComponent health;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
        if (health == null) return;

        health.OnDamaged.AddListener(HandleDamage);
        health.OnDeath.AddListener(HandleDestroyed);
    }

    private void OnDestroy()
    {
        if (health == null) return;

        health.OnDamaged.RemoveListener(HandleDamage);
        health.OnDeath.RemoveListener(HandleDestroyed);
    }

    private void HandleDamage(float damage)
    {
        int amount = Mathf.FloorToInt(damage * amountPerDamage);
        if (amount > 0)
        {
            TryGiveToLastAttacker(amount); // See note below
        }
    }

    private void HandleDestroyed()
    {
        TryGiveToLastAttacker(onDestroyBonus);
    }

    // Replace this with however you track the attacker
    private void TryGiveToLastAttacker(int amount)
    {
        var player = health.lastAttacker;
        if (player != null && player.TryGetComponent<IInventory>(out var provider))
        {
            provider.AddItem(itemData.itemID, amount);
        }
    }
}