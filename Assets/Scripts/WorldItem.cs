using System;
using Core;
using EventManager;
using InventorySystem;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : NetworkBehaviour, IInteractable, IItemContainer
{
    public class StoreableEventArgs : EventArgs
    {
        public WorldItem worldItem;
        public Actor actor;
        public Actor owner;
        public PlayerInventory inventory { get; set; }
    }

    public Actor actor;
    public readonly NetworkVariable<ItemAccessor> item = new();
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

    private void OnItemValueChanged(ItemAccessor previousvalue, ItemAccessor newvalue)
    {
        SpawnVisuals();
    }

    [Button]
    public void SetItemData(Item item) => this.item.Value = item.accessId;
    public ItemAccessor GetItemData() => item.Value;

    private void SpawnVisuals()
    {
        DestroyVisuals();
        if (item.Value.IsValid())
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
            worldItem = this,
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

    public void DepositImplementation(ItemAccessor itemAccessId)
    {
        this.item.Value = itemAccessId;
    }

    public void WithdrawImplementation(ItemAccessor itemAccessId)
    {
        this.item.Value = ItemAccessor.Empty;
        GameManager.Spawner.Despawn(this);
    }
}