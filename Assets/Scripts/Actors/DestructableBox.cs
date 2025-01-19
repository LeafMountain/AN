using System;
using Core;
using DG.Tweening;
using EventManager;
using UnityEngine;
using UnityEngine.Serialization;

public class DestructableBox : Actor
{
    [FormerlySerializedAs("damageReciever")] public DamageReceiver damageReceiver;

    protected override void Start()
    {
        base.Start();
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
    }

    protected override void OnDamaged(object origin, EventArgs eventargs)
    {
        if (damageReceiver.currentHealth> 0)
        {
            transform.DOPunchScale(Vector3.one * .1f, .2f);
        }
        else
        {
            transform.DOScale(Vector3.one * 2f, .1f).onComplete += () => Destroy(gameObject); 
        }
    }
}
