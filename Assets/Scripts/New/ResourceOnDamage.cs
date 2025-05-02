using UnityEngine;

public class ResourceOnDamage : MonoBehaviour
{
    [SerializeField] private string itemId = "rock";
    [SerializeField] private float amountPerDamage = 0.2f;
    [SerializeField] private int onDestroyBonus = 5;

    private HealthComponent health;

    // private void Awake()
    // {
    //     health = GetComponent<HealthComponent>();
    //     health.OnDamaged += HandleDamage;
    //     health.OnDied += HandleDestroyed;
    // }
    //
    // private void OnDestroy()
    // {
    //     if (health != null)
    //     {
    //         health.OnDamaged -= HandleDamage;
    //         health.OnDied -= HandleDestroyed;
    //     }
    // }
    //
    // private void HandleDamage(float damage, GameObject source)
    // {
    //     int amount = Mathf.FloorToInt(damage * amountPerDamage);
    //     if (amount > 0)
    //         TryGiveToInventory(source, amount);
    // }
    //
    // private void HandleDestroyed(GameObject source)
    // {
    //     TryGiveToInventory(source, onDestroyBonus);
    // }
    //
    // private void TryGiveToInventory(GameObject source, int amount)
    // {
    //     if (source.TryGetComponent<IInventoryProvider>(out var provider))
    //     {
    //         var inventory = provider.GetInventory();
    //         inventory?.AddItem(itemId, amount);
    //     }
    // }
}