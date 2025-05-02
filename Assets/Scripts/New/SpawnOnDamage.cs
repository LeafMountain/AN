using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class SpawnOnDamage : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool randomYRotation = true;

    private void Awake()
    {
        var health = GetComponent<HealthComponent>();
        health.OnDamaged.AddListener(OnDamaged);
    }

    private void OnDamaged(float damage)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 position = (spawnPoint != null ? spawnPoint.position : transform.position) + new Vector3(offset.x, 0f, offset.y);
            float yRot = randomYRotation ? Random.Range(0f, 360f) : 0f;
            Quaternion rotation = Quaternion.Euler(0f, yRot, 0f);

            Instantiate(spawnPrefab, position, rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = spawnPoint != null ? spawnPoint.position : transform.position;
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.4f);
        Gizmos.DrawWireSphere(center, spawnRadius);
    }
}