using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using EventManager;
using Mirror;
using UnityEngine;

public class Player : Character
{
    public int supplies = 0;
    public int maxEnergy = 100;
    public int energy = 100;
    public float lootRange = 2f;
    public float lootSpeed = 5f;

    private Collider[] colliders = new Collider[10];
    public LayerMask actorMask;
    private Tweener lootTween;

    private bool dead = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(EnergyTick());

        Events.AddListener(Flag.DamageRecieved, this, OnDamageReveived);
    }

    private async void OnDamageReveived(object origin, EventArgs eventargs)
    {
        if(dead) return;
        var damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed)
        {
            dead = true;
            await Task.Delay(5000);
            Reset();
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = Vector3.zero;
    }

    public IEnumerator EnergyTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            energy -= 1;
        }
    }

    public void FixedUpdate()
    {
        if (isServer)
        {
            LootMagnet();
        }
    }

    private void LootMagnet()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, lootRange, colliders, actorMask);
        for (int i = 0; i < hits; i++)
        {
            var actor = colliders[i].GetComponent<Actor>();
            var storeable = actor.GetComponent<Storeable>();
            float distance = Vector3.Distance(transform.position + Vector3.up, actor.transform.position);
            if (distance < 1f)
            {
                storeable.PickUp(this);
                NetworkServer.Destroy(storeable.gameObject);
                Destroy(storeable.gameObject);
            }
            else
            {
                actor.GetComponent<Rigidbody>().isKinematic = true;
                storeable.transform.position += (transform.position + Vector3.up - actor.transform.position).normalized * lootSpeed * Time.fixedDeltaTime;
            }
        }
    }

    public void AddEnergy(int value)
    {
        energy += value;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    protected override void Reset()
    {
        base.Reset();

        damageReciever.Reset();
        supplies = 0;
        energy = 100;
        dead = false;
    }
}