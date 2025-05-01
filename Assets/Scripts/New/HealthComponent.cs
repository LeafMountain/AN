using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class FloatEvent : UnityEvent<float> { }

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    public UnityEvent OnDeath = new();
    public FloatEvent OnDamaged = new();
    public FloatEvent OnHealed = new();

    private void Awake() => CurrentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        OnDamaged.Invoke(amount);

        if (CurrentHealth <= 0)
            OnDeath.Invoke();
    }

    public void Heal(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealed.Invoke(amount);
    }

    public void ResetHealth() => CurrentHealth = maxHealth;
}