using System;
using Core;
using EventManager;
using InventorySystem;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Storeable : NetworkBehaviour, IInteractable, IItemContainer
{
    public class StoreableEventArgs : EventArgs
    {
        public Storeable storeable;
        public Actor actor;
        public Actor owner;
        public PlayerInventory inventory { get; set; }
    }

    public Actor actor;
    public readonly NetworkVariable<int> item = new();
    private GameObject spawnedVisual;
    
    private void Awake()
    {
        item.OnValueChanged += OnItemValueChanged;
    }

    private void OnEnable()
    {
        SpawnVisuals();
    }

    private void OnDisable()
    {
        DestroyVisuals();
    }

    private void OnItemValueChanged(int previousvalue, int newvalue)
    {
        SpawnVisuals();
    }

    [Button]
    public void SetItemData(Item item)
    {
        this.item.Value = item.accessId;
    }

    public int GetItemData()
    {
        return item.Value;
    }

    private void SpawnVisuals()
    {
        DestroyVisuals();
        if (item.Value != 0)
        {
            Item? item = GameManager.ItemManager.GetItem(this.item.Value);
            ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
            itemData.graphics.InstantiateAsync(transform.position, transform.rotation, transform).Completed += handle => spawnedVisual = handle.Result;
        }
    }

    private void DestroyVisuals()
    {
        if (spawnedVisual)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                DestroyImmediate(spawnedVisual);
            }
            else
#endif
            {
                Destroy(spawnedVisual);
            }
        }
    }

    public void Interact(Actor interactor)
    {
        if (interactor.TryGetComponent(out PlayerInventory inventory) == false) return;

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

    public void DepositImplementation(int itemAccessId)
    {
        this.item.Value = itemAccessId;
    }

    public void WithdrawImplementation(int itemAccessId)
    {
        this.item.Value = 0;
        GameManager.Spawner.Despawn(this);
    }
}