using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<IDamageable>();
        if (health != null)
        {
            health.TakeDamage(damageAmount, gameObject);
        }
    }
}