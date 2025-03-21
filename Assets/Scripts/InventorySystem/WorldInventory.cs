using System.Collections.Generic;
using Core;
using DG.Tweening;
using Mirror;
using UnityEngine;

namespace InventorySystem {
    public class WorldInventory : NetworkBehaviour, IItemContainer {
        public InventoryHandle InventoryHandle {
            get => inventoryHandle;
            set => inventoryHandle = value;
        }

        [SyncVar] public InventoryHandle inventoryHandle = new();

        readonly List<GameObject> spawnedVisuals = new();

        public List<Transform> slots = new();

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
                    transform.DOShakeRotation(.1f, 5f);
                    break;
                case InventoryEventArgs.Operation.Withdrawn:
                    SpawnVisuals();
                    transform.DOShakeRotation(.1f, 5f);
                    break;
                case InventoryEventArgs.Operation.InventoryCreated:
                    SpawnVisuals();
                    Debug.Log("Inventory created");
                    break;
            }
        }

        void SpawnVisuals() {
            List<ActorHandle> items = GameManager.ItemManager.GetItems(InventoryHandle);
            DestroyVisuals();
            Debug.Log(items[0].id);

            for (int i = 0; i < items.Count; i++) {
                ActorHandle actorHandle = items[i];
                Item? item = actorHandle.GetItem();
                Transform slot = slots.Count > i ? slots[i] : transform;
                if (item.HasValue) {
                    ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                    itemData.graphics.InstantiateAsync(slot.position, slot.rotation, slot).Completed += handle => {
                        handle.Result.transform.localScale = Vector3.one;
                        spawnedVisuals.Add(handle.Result);
                    };
                }
            }
        }

        void DestroyVisuals() {
            if (spawnedVisuals.Count != 0) {
                for (int i = spawnedVisuals.Count - 1; i >= 0; i--) {
#if UNITY_EDITOR
                    if (Application.isPlaying == false) {
                        DestroyImmediate(spawnedVisuals[i]);
                    }
                    else
#endif
                    {
                        Destroy(spawnedVisuals[i]);
                    }

                    spawnedVisuals.RemoveAt(i);
                }
            }
        }
    }
}