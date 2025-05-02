using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;
    
    [Header("Spawn Area")]
    [SerializeField] private bool useSpawnPoints = false;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float radius = 5f;
    [SerializeField] private bool randomYRotation = true;

    private float timer;
    private readonly List<GameObject> aliveEnemies = new();

    private void Update()
    {
        timer += Time.deltaTime;

        aliveEnemies.RemoveAll(e => e == null);

        if (timer >= spawnInterval && aliveEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 position;
        Quaternion rotation;

        if (useSpawnPoints && spawnPoints.Length > 0)
        {
            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            position = point.position;
            rotation = point.rotation;
        }
        else
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            position = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            float yRot = randomYRotation ? Random.Range(0f, 360f) : transform.rotation.eulerAngles.y;
            rotation = Quaternion.Euler(0f, yRot, 0f);
        }

        var enemy = Instantiate(enemyPrefab, position, rotation);
        aliveEnemies.Add(enemy);

        var health = enemy.GetComponent<HealthComponent>();
        if (health != null)
            health.OnDeath.AddListener(() => aliveEnemies.Remove(enemy));
    }

    private void OnDrawGizmosSelected()
    {
        if (!useSpawnPoints)
        {
            Gizmos.color = new Color(1f, 0.4f, 0.4f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        else
        {
            Gizmos.color = new Color(0.4f, 1f, 0.4f, 0.6f);
            foreach (var point in spawnPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }
}
