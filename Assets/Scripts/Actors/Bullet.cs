using System;
using System.Collections;
using EventManager;
using Mirror;
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
    private float speed = 1f;
    private IEnumerator lifetimeTimer;
    private bool isDestroying;
    
    public ParticleSystem impactEffect;

    private void FixedUpdate()
    {
        if(isServer == false) return;
        
        if(isDestroying) return;
        
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
                if (hit.transform.TryGetComponent(out DamageReciever damageReciever))
                    damageReciever.DoDamage(this);
                
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                DestroyBullet();
            }
        }
    }

    public void Init(Gun owner, float speed, float lifetime = 30f)
    {
        this.owner = owner;
        this.speed = speed;
        lifetimeTimer = LifetimeTimer(lifetime);
        StartCoroutine(lifetimeTimer);
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
        if(isDestroying) return;
        GetComponent<MeshRenderer>().enabled = false;
        isDestroying = true;
        if(lifetimeTimer != null) StopCoroutine(lifetimeTimer);
        Destroy(gameObject, 2f);
        NetworkServer.Destroy(gameObject);
    }
}