using System;
using System.Linq;
using Core;
using EffectSystem;
using EventManager;
using Sirenix.Serialization;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Actor))]
public class DamageReceiver : NetworkActorComponent
{
    public int maxHealth = 10;
    public NetworkVariable<int> currentHealth;
    public bool autoDestroy = true;

    public int lootCount = 1;
    public Loot[] loot;
    private float lootSpawnForce = 5f;
    public bool IsDead => currentHealth.Value == 0;

    [NonSerialized, OdinSerialize] public Effect[] customDestructionEffects = Array.Empty<Effect>();
    public EffectData destructionEffect;
    public EffectData hitEffect;
    public Vector3 lastHitPoint;
    public Vector3 lastHitNormal;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    public virtual void DoDamage(int damage, Actor instigator)
    {
        currentHealth.Value = math.clamp(currentHealth.Value - damage, 0, int.MaxValue);
        Events.TriggerEvent(Flag.DamageRecieved, Parent, new DamageRecievedArgs()
        {
            reciever = Parent,
            damageReceiver = this,
            damage = damage,
            instigator = instigator,
            destroyed = currentHealth.Value == 0,
        });

        if (IsServer)
        {
            if (autoDestroy && currentHealth.Value == 0)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();

                for (int i = 0; i < lootCount; i++)
                {
                    Loot spawnedLoot = Instantiate(loot[Random.Range(0, loot.Length)], transform.position,
                        transform.rotation);
                    Vector3 insideUnitSphere = Random.insideUnitSphere;
                    insideUnitSphere.y = Mathf.Abs(insideUnitSphere.y);
                    spawnedLoot.GetComponent<Rigidbody>()
                        .AddForce(insideUnitSphere * lootSpawnForce, ForceMode.VelocityChange);
                    spawnedLoot.GetComponent<NetworkObject>().Spawn();
                }

                if (destructionEffect != null)
                {
                    destructionEffect.PlayEffect(gameObject, gameObject, transform.position, transform.rotation);
                }
            }
        }
    }

    public virtual void DoDamage(Bullet bullet)
    {
        currentHealth.Value = math.clamp(currentHealth.Value - 1, 0, int.MaxValue);
        Events.TriggerEvent(Flag.DamageRecieved, Parent, new DamageRecievedArgs()
        {
            reciever = Parent,
            damageReceiver = this,
            damage = 1,
            instigator = bullet.owner,
            destroyed = currentHealth.Value == 0,
        });

        if (customDestructionEffects.Any())
        {
            EffectManager.PlayEffect(customDestructionEffects, gameObject, gameObject, bullet.transform.position,
                Quaternion.LookRotation(-bullet.transform.forward));
        }

        if (destructionEffect)
        {
            hitEffect.PlayEffect(gameObject, gameObject, bullet.transform.position,
                Quaternion.LookRotation(-bullet.transform.forward));
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
                    Loot spawnedLoot = Instantiate(loot[Random.Range(0, loot.Length)], transform.position,
                        transform.rotation);
                    Vector3 insideUnitSphere = Random.insideUnitSphere;
                    insideUnitSphere.y = Mathf.Abs(insideUnitSphere.y);
                    spawnedLoot.GetComponent<Rigidbody>()
                        .AddForce(insideUnitSphere * lootSpawnForce, ForceMode.VelocityChange);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2)
        {
            DoDamage((int)(collision.relativeVelocity.magnitude * .5f), Parent);
        }
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