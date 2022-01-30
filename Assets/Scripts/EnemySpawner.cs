using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public class EnemySpawner : Actor 
{
    public Enemy enemy;

    public float spawnRate;
    public int maxEnemiesSpawned = 5;

    public List<Enemy> spawnedEnemies = new List<Enemy>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        SpawnTick(); 
    }

    private async void SpawnTick()
    {
        while (Application.isPlaying)
        {
            await Task.Delay((int)spawnRate * 1000);
            
            if(spawnedEnemies.Count < maxEnemiesSpawned)
                Spawn();    
            
            for (var i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if(spawnedEnemies[i] == null)
                    spawnedEnemies.RemoveAt(i);
            }
        } 
    }

    public void Spawn()
    {
        var enemy = Instantiate(this.enemy, transform.position, transform.rotation);
        NetworkServer.Spawn(enemy.gameObject);
        spawnedEnemies.Add(enemy); 
    }
}
