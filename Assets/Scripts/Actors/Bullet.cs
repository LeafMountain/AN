using System;
using System.Collections;
using EffectSystem;
using EventManager;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public class BulletImpactArgs : EventArgs
    {
        public enum EventType
        {
            None,
            Impact,
            NoImpact
        }

        public EventType eventType;
        public Gun owner;
        public RaycastHit hit;
    }

    public Gun owner;
    protected float speed = 1f;
    private IEnumerator lifetimeTimer;
    private bool isDestroying;

    public EffectData impactEffect;
    protected Vector3 targetPosition;
    
    public void Init(Gun owner, float speed, Vector3 targetPosition, float lifetime = 30f)
    {
        this.owner = owner;
        this.speed = speed;
        lifetimeTimer = LifetimeTimer(lifetime);
        this.targetPosition = targetPosition;
        StartCoroutine(lifetimeTimer);
    }

    protected virtual void FixedUpdate()
    {
        if (IsServer == false) return;

        if (isDestroying) return;

        Vector3 step = Vector3.forward * speed * Time.fixedDeltaTime;
        float stepDistance = step.magnitude;
        transform.Translate(step, Space.Self);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            if (hit.distance < stepDistance)
            {
                // Hit next frame!
                Events.TriggerEvent(Flag.BulletImpact, this, new BulletImpactArgs()
                {
                    owner = owner,
                    hit = hit,
                    eventType = BulletImpactArgs.EventType.Impact
                });
                
                if (hit.transform.TryGetComponent(out DamageReceiver damageReciever))
                {}
                    // damageReciever.DoDamage(this);

                // Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                // DestroyBullet();
                
                OnCollision(hit.point, hit.normal, damageReciever);
            }
        }
    }

    protected virtual void OnCollision(Vector3 position, Vector3 normal, DamageReceiver damageReceiver)
    {
        if (damageReceiver) damageReceiver.DoDamage(this);
        impactEffect.PlayEffect(gameObject, gameObject, position, Quaternion.LookRotation(normal));
        DestroyBullet();
    }

    private IEnumerator LifetimeTimer(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Events.TriggerEvent(Flag.BulletImpact, this, new BulletImpactArgs
        {
            owner = owner,
            eventType = BulletImpactArgs.EventType.NoImpact
        });
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (isDestroying) return;
        if(TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        
        isDestroying = true;
        if (lifetimeTimer != null) StopCoroutine(lifetimeTimer);
        // Destroy(gameObject, 2f);
        GetComponent<NetworkObject>().Despawn();
    }
}