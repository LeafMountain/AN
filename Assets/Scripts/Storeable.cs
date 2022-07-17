using System;
using EventManager;
using InventorySystem;
using UnityEngine;
using UnityEngine.UIElements;

public class Storeable : MonoBehaviour, IInteractable 
{
    public class StoreableEventArgs : EventArgs
    {
        public Storeable storeable;
        public Actor actor;
        public Actor owner;
        public Inventory inventory { get; set; }
    }

    public enum ItemType
    {
        None,
        Supply,
        Battery
    }

    public ItemType itemType;
    public Actor actor;

    public Inventory.ItemData itemData;

    public void Interact(Actor interactor)
    {
        if(interactor.TryGetComponent(out Inventory inventory) == false) return;
        
        Events.TriggerEvent(Flag.Storeable, actor, new StoreableEventArgs
        {
            storeable = this,
            actor = actor,
            owner = actor,
            inventory = inventory
        });

        GameManager.Despawn(gameObject);
    }

    public string GetPrompt()
    {
        return "Pick Up";
    }
}