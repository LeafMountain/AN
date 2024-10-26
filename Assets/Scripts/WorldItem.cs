using System;
using Core;
using EventManager;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : NetworkBehaviour, IInteractable, IItemContainer {
    public InventoryHandle InventoryHandle { get; set; }

    public class StoreableEventArgs : EventArgs {
        public WorldItem worldItem;
        public Actor actor;
        public Actor owner;
        public PlayerInventory inventory { get; set; }
    }

    public Actor actor;
    GameObject spawnedVisual;

    public override void OnNetworkSpawn() {
        if (HasAuthority) {
            if (InventoryHandle.IsValid() == false) {
                InventoryHandle = GameManager.ItemManager.CreateInventory();
            }
        }

        GameManager.ItemManager.AddCallback(InventoryHandle, OnInventoryCallback);
        SpawnVisuals();
    }

    void OnInventoryCallback(object sender, InventoryEventArgs args) {
        if (args.operation == InventoryEventArgs.Operation.Deposited) {
            SpawnVisuals();
        }
        else if (args.operation == InventoryEventArgs.Operation.Withdrawn) {
            DestroyVisuals();
            GameManager.Spawner.Despawn(this);
        }
    }

    void OnEnable() {
        if (InventoryHandle.IsValid()) {
            SpawnVisuals();
        }
    }

    void OnDisable() {
        DestroyVisuals();
    }

    public override void OnDestroy() {
        if (Application.isPlaying == false) return;
        GameManager.ItemManager.RemoveCallback(InventoryHandle, OnInventoryCallback);
        GameManager.ItemManager.ReturnInventory(InventoryHandle);
    }

    public ItemHandle GetItemHandle() => GameManager.ItemManager.GetItems(InventoryHandle)[0];

    void SpawnVisuals() {
        DestroyVisuals();
        ItemHandle itemHandle = GetItemHandle();
        if (itemHandle.IsValid()) {
            Item? item = GameManager.ItemManager.GetItem(itemHandle);
            ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
            itemData.graphics.InstantiateAsync(transform.position, transform.rotation, transform).Completed +=
                handle => spawnedVisual = handle.Result;
        }
    }

    void DestroyVisuals() {
        if (spawnedVisual) {
#if UNITY_EDITOR
            if (Application.isPlaying == false) {
                DestroyImmediate(spawnedVisual);
            }
            else
#endif
            {
                Destroy(spawnedVisual);
            }
        }
    }

    public void Interact(Actor interactor) {
        if (interactor.TryGetComponent(out PlayerInventory inventory) == false) return;

        Events.TriggerEvent(Flag.Storeable, actor, new StoreableEventArgs {
            worldItem = this,
            actor = actor,
            owner = actor,
            inventory = inventory
        });

        GameManager.Spawner.Despawn(gameObject);
    }

    public string GetPrompt() {
        return "Pick Up";
    }
}