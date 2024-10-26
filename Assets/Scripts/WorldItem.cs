using System;
using Core;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : NetworkBehaviour, IInteractable, IItemContainer {
    public InventoryHandle InventoryHandle {
        get => inventoryHandle.Value;
        set => inventoryHandle.Value = value;
    }

    readonly NetworkVariable<InventoryHandle> inventoryHandle = new();
    GameObject spawnedVisual;

    protected override void OnNetworkPostSpawn() {
        if (IsServer && InventoryHandle.IsValid() == false) {
            InventoryHandle = GameManager.ItemManager.CreateInventory();
        }
        
        GameManager.ItemManager.AddCallback(InventoryHandle, OnInventoryCallback);
        
        if (GameManager.ItemManager.IsInventoryInitialized(InventoryHandle)) {
            SpawnVisuals();
        }
    }

    public override void OnNetworkDespawn() {
        GameManager.ItemManager.RemoveCallback(InventoryHandle, OnInventoryCallback);
        
        if (IsServer) {
            GameManager.ItemManager.ReturnInventory(InventoryHandle);
            InventoryHandle = default;
        }
    }

    void OnInventoryCallback(object sender, InventoryEventArgs args) {
        switch (args.operation) {
            case InventoryEventArgs.Operation.Deposited:
                SpawnVisuals();
                break;
            case InventoryEventArgs.Operation.Withdrawn:
                DestroyVisuals();
                GameManager.Spawner.Despawn(this);
                break;
            case InventoryEventArgs.Operation.InventoryCreated:
                SpawnVisuals();
                break;
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
        GameManager.Spawner.Despawn(gameObject);
    }

    public string GetPrompt() {
        return "Pick Up";
    }
}