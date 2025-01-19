using Core;
using InventorySystem;
using Mirror;
using UnityEngine;

public class WorldItem : NetworkBehaviour, IInteractable, IItemContainer {
    public InventoryHandle InventoryHandle {
        get => inventoryHandle;
        set => inventoryHandle = value;
    }

    [SyncVar] InventoryHandle inventoryHandle;
    GameObject spawnedVisual;

    public override void OnStartClient() {
        if (NetworkServer.active && InventoryHandle.IsValid() == false) {
            InventoryHandle = GameManager.ItemManager.CreateInventory();
        }

        GameManager.ItemManager.AddCallback(InventoryHandle, OnInventoryCallback);

        if (GameManager.ItemManager.IsInventoryInitialized(InventoryHandle)) {
            SpawnVisuals();
        }
    }

    public override void OnStopClient() {
        GameManager.ItemManager.RemoveCallback(InventoryHandle, OnInventoryCallback);

        if (NetworkServer.active) {
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

    public ActorHandle GetItemHandle() => GameManager.ItemManager.GetItems(InventoryHandle)[0];

    void SpawnVisuals() {
        DestroyVisuals();
        ActorHandle actorHandle = GetItemHandle();
        if (actorHandle.IsValid()) {
            Item? item = GameManager.ItemManager.GetItem(actorHandle);
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