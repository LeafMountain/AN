using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private HealthComponent health;

    public void Respawn()
    {
        transform.position = respawnPoint.position;
        health.ResetHealth();
    }
}