using System;
using EffectSystem;
using EventManager;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Actor))]
public class DamageReceiver : NetworkBehaviour
{
    public Actor actor;
    public int maxHealth = 10;
    public NetworkVariable<int> currentHealth;
    public bool autoDestroy = true;

    public int lootCount = 1;
    public Loot[] loot;
    private float lootSpawnForce = 5f;
    public bool IsDead => currentHealth.Value == 0;
    
    public EffectData destructionEffect;
    public EffectData hitEffect;
    public Vector3 lastHitPoint;
    public Vector3 lastHitNormal;

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
            damageReceiver = this,
            damage = 1,
            instigator = bullet.owner,
            destroyed = currentHealth.Value == 0,
        });

        if (destructionEffect)
        {
            hitEffect.PlayEffect(gameObject, gameObject, bullet.transform.position, Quaternion.LookRotation(-bullet.transform.forward));
        }

        lastHitPoint = bullet.transform.position;
        lastHitNormal = -bullet.transform.forward;

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

                if (destructionEffect != null)
                {
                    destructionEffect.PlayEffect(gameObject, gameObject, transform.position, transform.rotation);
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
    public DamageReceiver damageReceiver;
    public int damage;
    public Actor instigator;
    public bool destroyed;
}