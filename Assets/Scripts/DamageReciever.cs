using System;
using EventManager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Actor))]
public class DamageReciever : NetworkBehaviour
{
    public Actor actor;
    public int maxHealth = 10;
    public NetworkVariable<int> currentHealth;
    [FormerlySerializedAs("destructable")] public bool autoDestroy = true;

    public int lootCount = 1;
    public Loot[] loot;
    private float lootSpawnForce = 5f;
    public bool IsDead => currentHealth.Value == 0;

    private void OnValidate()
    {
        actor = GetComponent<Actor>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    public virtual void DoDamage(Bullet bullet)
    {
        currentHealth.Value--;
        Events.TriggerEvent(Flag.DamageRecieved, actor, new DamageRecievedArgs()
        {
            reciever = actor,
            damageReceiver= this,
            damage = 1,
            instigator = bullet.owner,
            destroyed = currentHealth.Value == 0,
        });

        if (IsServer)
        {
            if (autoDestroy && currentHealth.Value == 0)
            { 
                gameObject.GetComponent<NetworkObject>().Despawn();
                
                for (int i = 0; i < lootCount; i++)
                {
                    Loot spawnedLoot = Instantiate(loot[Random.Range(0, loot.Length)], transform.position, transform.rotation);
                    Vector3 insideUnitSphere = Random.insideUnitSphere;
                    insideUnitSphere.y = Mathf.Abs(insideUnitSphere.y);
                    spawnedLoot.GetComponent<Rigidbody>().AddForce(insideUnitSphere * lootSpawnForce, ForceMode.VelocityChange);
                    
                    spawnedLoot.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }

    public void Reset()
    {
        currentHealth.Value = maxHealth;
    }
}

public class DamageRecievedArgs : EventArgs
{
    public Actor reciever;
    public DamageReciever damageReceiver;
    public int damage;
    public Actor instigator;
    public bool destroyed;
}