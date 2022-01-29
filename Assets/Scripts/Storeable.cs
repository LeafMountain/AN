using System;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using UnityEngine;

public class Storeable : MonoBehaviour
{
    public class StoreableEventArgs : EventArgs
    {
        public Storeable storeable;
        public Actor actor;
        public Actor owner;
    }

    public enum ItemType
    {
        None,
        Supply,
        Battery
    }

    public ItemType itemType;
    public Actor actor;

    public virtual void PickUp(Actor actor)
    {
        Events.TriggerEvent(Flag.Storeable, actor, new StoreableEventArgs
        {
            storeable = this,
            actor = this.actor,
            owner = actor
        });
    }
}