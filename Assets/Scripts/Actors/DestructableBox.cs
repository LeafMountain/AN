using System;
using DG.Tweening;
using EventManager;
using UnityEngine;

public class DestructableBox : Actor
{
    public DamageReciever damageReciever;

    protected override void Start()
    {
        base.Start();
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
    }

    private void OnDamaged(object origin, EventArgs eventargs)
    {
        if (damageReciever.currentHealth > 0)
        {
            transform.DOPunchScale(Vector3.one * .1f, .2f);
        }
        else
        {
            transform.DOScale(Vector3.one * 2f, .1f).onComplete += () => Destroy(gameObject); 
        }
    }
}
