using System;
using EventManager;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageReciever : NetworkBehaviour
{
    public Actor actor;
    public int maxHealth = 10;
    [SyncVar] public int currentHealth = 0;
    public bool destructable = true;

    public int lootCount = 1;
    public GameObject loot;
    private float lootSpawnForce = 5f;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
    }

    public virtual void DoDamage(Bullet bullet)
    {
        currentHealth--;
        Events.TriggerEvent(Flag.DamageRecieved, actor, new DamageRecievedArgs()
        {
            reciever = actor,
            damage = 1,
            instigator = bullet.owner,
            destroyed = currentHealth == 0,
        });

        if (isServer)
        {
            if (destructable && currentHealth == 0)
            {
                for (int i = 0; i < lootCount; i++)
                {
                    NetworkServer.Destroy(gameObject);
                    GameObject spawnedLoot = Instantiate(loot, transform.position, transform.rotation);
                    NetworkServer.Spawn(spawnedLoot);
                    Vector3 insideUnitSphere = Random.insideUnitSphere;
                    insideUnitSphere.y = Mathf.Abs(insideUnitSphere.y);
                    spawnedLoot.GetComponent<Rigidbody>().AddForce(insideUnitSphere * lootSpawnForce, ForceMode.VelocityChange);
                }
            }
        }
    }
}

public class DamageRecievedArgs : EventArgs
{
    public Actor reciever;
    public int damage;
    public Actor instigator;
    public bool destroyed;
}