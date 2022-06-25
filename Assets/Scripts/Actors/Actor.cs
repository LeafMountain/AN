using System;
using DG.Tweening;
using EventManager;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : NetworkBehaviour 
{
    public UIActor ui;

    protected virtual void Start()
    {
        if (ui)
        {
            Vector3 position = GetCenterOfMass() + Vector3.up * 1.5f;
            Quaternion rotation = transform.rotation;
            ui = GameManager.Spawn(ui, position, rotation, transform);
            ui.Init(this);
        }
        
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
    }

    protected virtual void Update()
    {
    }

    protected virtual void Reset()
    {
         
    }

    public virtual Vector3 GetCenterOfMass()
    {
        Vector3 centerOfMass = GetComponent<Collider>().bounds.center;
        return centerOfMass;
    }
    
    protected virtual void OnDamaged(object origin, EventArgs eventargs)
    {
        var args = eventargs as DamageRecievedArgs;
        if (args.damageReceiver.currentHealth.Value > 0)
        {
            transform.DOPunchScale(Vector3.one * .1f, .2f);
        }
        else
        {
            transform.DOScale(Vector3.one * 2f, .1f).onComplete += () => Destroy(gameObject);
        }
    }
    
}
