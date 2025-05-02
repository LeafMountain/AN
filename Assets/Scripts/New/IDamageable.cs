using UnityEngine;

public interface IDamageable {
    float CurrentHealth { get; } 
    float MaxHealth { get; }
    void TakeDamage(float amount, GameObject source);
    void Heal(float amount);
    void ResetHealth();
}
