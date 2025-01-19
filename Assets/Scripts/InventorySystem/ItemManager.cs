using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace InventorySystem {
    public class ItemManager : NetworkBehaviour {
        const int InventorySlots = 16;

        SyncList<Item> items;
        SyncList<ActorHandle> inventoryItems;
        SyncList<InventoryHandle> inventories;

        readonly Dictionary<ActorHandle, InventoryHandle> itemOwners = new();
        readonly Dictionary<InventoryHandle, IItemContainer> itemContainers = new();
        readonly Dictionary<InventoryHandle, EventHandler<InventoryEventArgs>> inventoryCallbacks = new();

        void Awake() {
            items = new SyncList<Item>();
            inventoryItems = new SyncList<ActorHandle>();
            inventories = new SyncList<InventoryHandle>();

            // items.Initialize(this);
            // inventoryItems.Initialize(this);
            // inventories.Initialize(this);
        }

        public override void OnStartClient() {
            inventories.OnAdd += OnAdd;
            inventories.OnInsert += OnAdd;

            // inventoryItems.OnListChanged += InventoryItemsOnOnListChanged;

            foreach (InventoryHandle inventoryHandle in inventories) {
                InvokeInventoryCreatedEvent(inventoryHandle);
            }
        }

        void OnAdd(int index) {
            // int inventoryId = Mathf.FloorToInt(index.Index / InventorySlots);
            //
            // // If the inventory doesnt exist anymore, assume no one is listening?
            // if (inventories.Count <= inventoryId) {
            //     itemOwners.Remove(changeevent.PreviousValue);
            //     return;
            // }
            //
            // InventoryHandle inventoryHandle = inventories[inventoryId];
            //
            // // Withdrew
            // if (changeevent.PreviousValue.IsValid()) {
            //     itemOwners.Remove(changeevent.PreviousValue);
            //     if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
            //         inventoryCallbacks[inventoryHandle].Invoke(this, new InventoryEventArgs() {
            //             ActorHandle = changeevent.PreviousValue,
            //             inventoryHandle = inventoryHandle,
            //             operation = InventoryEventArgs.Operation.Withdrawn,
            //         });
            //     }
            // }
            //
            // // Deposited
            // if (changeevent.Value.IsValid()) {
            //     itemOwners[changeevent.Value] = inventories[inventoryId];
            //     if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
            //         inventoryCallbacks[inventoryHandle].Invoke(changeevent.PreviousValue, new InventoryEventArgs() {
            //             ActorHandle = changeevent.Value,
            //             inventoryHandle = inventoryHandle,
            //             operation = InventoryEventArgs.Operation.Deposited,
            //         });
            //     }
            // }
        }

        // void InventoryItemsOnOnListChanged(NetworkListEvent<ActorHandle> changeevent) {
        //     if (changeevent.Type is NetworkListEvent<ActorHandle>.EventType.Add or NetworkListEvent<ActorHandle>.EventType.Insert or NetworkListEvent<ActorHandle>.EventType.Value) {
        //         int inventoryId = Mathf.FloorToInt(changeevent.Index / InventorySlots);
        //
        //         // If the inventory doesnt exist anymore, assume no one is listening?
        //         if (inventories.Count <= inventoryId) {
        //             itemOwners.Remove(changeevent.PreviousValue);
        //             return;
        //         }
        //
        //         InventoryHandle inventoryHandle = inventories[inventoryId];
        //
        //         // Withdrew
        //         if (changeevent.PreviousValue.IsValid()) {
        //             itemOwners.Remove(changeevent.PreviousValue);
        //             if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
        //                 inventoryCallbacks[inventoryHandle].Invoke(this, new InventoryEventArgs() {
        //                     ActorHandle = changeevent.PreviousValue,
        //                     inventoryHandle = inventoryHandle,
        //                     operation = InventoryEventArgs.Operation.Withdrawn,
        //                 });
        //             }
        //         }
        //
        //         // Deposited
        //         if (changeevent.Value.IsValid()) {
        //             itemOwners[changeevent.Value] = inventories[inventoryId];
        //             if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
        //                 inventoryCallbacks[inventoryHandle].Invoke(changeevent.PreviousValue, new InventoryEventArgs() {
        //                     ActorHandle = changeevent.Value,
        //                     inventoryHandle = inventoryHandle,
        //                     operation = InventoryEventArgs.Operation.Deposited,
        //                 });
        //             }
        //         }
        //     }
        //     else if (changeevent.Type is NetworkListEvent<ActorHandle>.EventType.Remove or NetworkListEvent<ActorHandle>.EventType.RemoveAt) {
        //         int inventoryId = Mathf.FloorToInt(changeevent.Index / InventorySlots);
        //         InventoryHandle inventoryHandle = inventories[inventoryId];
        //         itemOwners.Remove(changeevent.Value);
        //
        //         if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
        //             inventoryCallbacks[inventoryHandle].Invoke(changeevent.PreviousValue, new InventoryEventArgs() {
        //                 ActorHandle = changeevent.PreviousValue,
        //                 inventoryHandle = inventoryHandle,
        //                 operation = InventoryEventArgs.Operation.Deposited,
        //             });
        //         }
        //     }
        // }
        //
        // void InventoriesOnOnListChanged(NetworkListEvent<InventoryHandle> changeevent) {
        //     InvokeInventoryCreatedEvent(changeevent.Value);
        // }

        void InvokeInventoryCreatedEvent(InventoryHandle inventoryHandle) {
            if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                inventoryCallbacks[inventoryHandle].Invoke(this, new InventoryEventArgs() {
                    ActorHandle = default,
                    inventoryHandle = inventoryHandle,
                    operation = InventoryEventArgs.Operation.InventoryCreated,
                });
            }
        }

        public InventoryHandle CreateInventory() {
            InventoryHandle inventoryHandle = new();

            inventoryHandle.id = inventories.Count + 1;
            for (int i = 0; i < inventories.Count; i++) {
                if (inventories[i].id == i + 1) continue;
                inventoryHandle.id = i + 1;
                break;
            }

            if (inventoryItems.Count < inventoryHandle.id * InventorySlots + InventorySlots) {
                for (int i = 0; i < InventorySlots; i++) {
                    inventoryItems.Add(default);
                }
            }

            inventories.Insert(inventoryHandle.id - 1, inventoryHandle);
            return inventoryHandle;
        }

        void AddInventory(InventoryHandle inventoryHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return;

            if (authority == false) {
                AddInventoryRpc(inventoryHandle);
                return;
            }

            inventories.Add(inventoryHandle);
        }

        [Command(requiresAuthority = false)]
        void AddInventoryRpc(InventoryHandle inventoryHandle) => AddInventory(inventoryHandle);

        public void ReturnInventory(InventoryHandle handle) {
            if (ValidateInventoryHandle(handle) == false) return;
            inventories.Remove(handle);
            // Clear items?
        }

        public InventoryHandle GetInventoryHandle(int id) {
            if (inventories.Count < id) {
                Debug.LogError("Inventory outside of range");
                return InventoryHandle.Empty;
            }

            return inventories[id];
        }

        public InventoryHandle GetInventoryHandle(ActorHandle actorHandle) {
            return itemOwners.GetValueOrDefault(actorHandle);
        }

        public List<ActorHandle> GetItems(InventoryHandle inventoryHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return null;

            List<ActorHandle> itemHandles = new(InventorySlots);
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                itemHandles.Add(inventoryItems[i]);
            }

            return itemHandles;
        }

        public IItemContainer GetItemContainer(InventoryHandle inventoryHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return null;
            return itemContainers[inventoryHandle];
        }

        [Command(requiresAuthority = false)]
        void AddItemRpc(Item item) => AddItem(item);

        void AddItem(Item item) {
            if (authority == false) {
                AddItemRpc(item);
                return;
            }

            items.Add(item);
        }

        public ActorHandle CreateItem(string slug) {
            Item item = Item.Create(slug);
            AddItemRpc(item);
            return item.Handle;
        }

        public Item? GetItem(ActorHandle id) {
            foreach (Item item in items) {
                if (item.Handle == id) {
                    return item;
                }
            }

            return default;
        }

        [Command(requiresAuthority = false)]
        void PlaceItemInWorldRpc(ActorHandle actorHandle, Vector3 spawnPosition, Quaternion spawnRotation) => PlaceItemInWorld(actorHandle, spawnPosition, spawnRotation);

        public void PlaceItemInWorld(ActorHandle actorHandle, Vector3 spawnPosition, Quaternion spawnRotation) {
            if (authority == false) {
                PlaceItemInWorldRpc(actorHandle, spawnPosition, spawnRotation);
                return;
            }

            Item? item = GetItem(actorHandle);
            if (item.HasValue == false) {
                Debug.LogError("Trying to spawn null item");
                return;
            }

            if (IsInInventory(actorHandle)) {
                Withdraw(actorHandle);
            }

            GameManager.Spawner.SpawnItem(item.Value, spawnPosition, spawnRotation);
        }

        public void PickUpItem(ActorHandle actorAccessId) {
            Item? item = GetItem(actorAccessId);
            if (item.HasValue == false) {
                Debug.LogError("Trying to despawn null item");
                return;
            }

            // GameManager.Spawner.Despawn(item.Value);
        }

        public bool IsInInventory(ActorHandle actorHandle) {
            return itemOwners.ContainsKey(actorHandle);
        }

        bool ValidateInventoryHandle(InventoryHandle inventoryHandle) {
            if (inventoryHandle.IsValid() == false) {
                Debug.LogError($"Inventory handle not valid. ID {inventoryHandle.id}");
                return false;
            }

            return true;
        }

        [Command(requiresAuthority = false)]
        void DepositRpc(InventoryHandle inventoryHandle, ActorHandle actorHandle) => Deposit(inventoryHandle, actorHandle);

        public void Deposit(InventoryHandle inventoryHandle, ActorHandle actorHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) {
                return;
            }

            if (authority == false) {
                DepositRpc(inventoryHandle, actorHandle);
                return;
            }

            if (IsInInventory(actorHandle)) {
                Withdraw(actorHandle);
            }

            bool successful = false;
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (inventoryItems[i].IsValid()) continue;
                inventoryItems[i] = actorHandle;
                successful = true;
                break;
            }

            if (successful == false) {
                Debug.LogError("Inventory Full");
            }
        }

        [Command(requiresAuthority = false)]
        void WithdrawRpc(ActorHandle actorHandle) => Withdraw(actorHandle);

        public void Withdraw(ActorHandle actorHandle) {
            if (authority == false) {
                WithdrawRpc(actorHandle);
                return;
            }

            InventoryHandle inventoryHandle = GetInventoryHandle(actorHandle);

            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (inventoryItems[i] != actorHandle) continue;
                inventoryItems[i] = default;
                break;
            }
        }

        public void AddCallback(InventoryHandle inventoryHandle, EventHandler<InventoryEventArgs> callback) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return;
            inventoryCallbacks.TryAdd(inventoryHandle, default);
            inventoryCallbacks[inventoryHandle] += callback;
        }

        public void RemoveCallback(InventoryHandle inventoryHandle, EventHandler<InventoryEventArgs> callback) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return;
            if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                inventoryCallbacks[inventoryHandle] -= callback;
                if (inventoryCallbacks[inventoryHandle] == null) {
                    inventoryCallbacks.Remove(inventoryHandle);
                }
            }
        }

        public bool IsInventoryInitialized(InventoryHandle inventoryHandle) {
            return inventories.Contains(inventoryHandle);
        }
    }
}