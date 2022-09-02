using System;
using BlockBuilder;
using Core;
using UnityEngine;

public class BlockGrid2 : Actor
{
    [SerializeField] private int health = 10;
    [SerializeField] private BlockGridHolder blockGridHolder;

    protected override void OnDamaged(object origin, EventArgs eventargs)
    {
        base.OnDamaged(origin, eventargs);

        var args = eventargs as DamageRecievedArgs;

        health--;
        if (health <= 0)
        {
            health = 10;
            blockGridHolder.RemoveBlock(args.damageReceiver.lastHitPoint);
        }
    }
}