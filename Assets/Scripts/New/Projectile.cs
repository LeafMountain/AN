using UnityEngine;

public class Projectile : MonoBehaviour, IOwnable {
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    public GameObject Owner { get; set; }

    private void Start() {
        if (Owner == null) {
            Owner = gameObject;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other) {
        var health = other.GetComponent<IDamageable>();
        if (health != null) {
            health.TakeDamage(damage, Owner);
        }

        Destroy(gameObject);
    }
}