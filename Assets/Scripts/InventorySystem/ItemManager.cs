using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem {
    public class ItemManager : NetworkBehaviour {
        const int InventorySlots = 16;

        NetworkList<Item> items;
        NetworkList<ItemHandle> inventoryItems;
        NetworkList<InventoryHandle> inventories;

        readonly Dictionary<ItemHandle, InventoryHandle> itemOwners = new();
        readonly Dictionary<InventoryHandle, IItemContainer> itemContainers = new();
        readonly Dictionary<InventoryHandle, EventHandler<InventoryEventArgs>> inventoryCallbacks = new();

        void Awake() {
            items = new NetworkList<Item>();
            inventoryItems = new NetworkList<ItemHandle>();
            inventories = new NetworkList<InventoryHandle>();

            items.Initialize(this);
            inventoryItems.Initialize(this);
            inventories.Initialize(this);
        }

        public override void OnNetworkSpawn() {
            inventories.OnListChanged += InventoriesOnOnListChanged;
            inventoryItems.OnListChanged += InventoryItemsOnOnListChanged;

            foreach (InventoryHandle inventoryHandle in inventories) {
                InvokeInventoryCreatedEvent(inventoryHandle);
            }
        }

        void InventoryItemsOnOnListChanged(NetworkListEvent<ItemHandle> changeevent) {
            if (changeevent.Type is NetworkListEvent<ItemHandle>.EventType.Add or NetworkListEvent<ItemHandle>.EventType.Insert or NetworkListEvent<ItemHandle>.EventType.Value) {
                int inventoryId = Mathf.FloorToInt(changeevent.Index / InventorySlots);
                
                // If the inventory doesnt exist anymore, assume no one is listening?
                if (inventories.Count <= inventoryId) {
                    itemOwners.Remove(changeevent.PreviousValue);
                    return;
                }
                
                InventoryHandle inventoryHandle = inventories[inventoryId];

                // Withdrew
                if (changeevent.PreviousValue.IsValid()) {
                    itemOwners.Remove(changeevent.PreviousValue);
                    if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                        inventoryCallbacks[inventoryHandle].Invoke(this, new InventoryEventArgs() {
                            itemHandle = changeevent.PreviousValue,
                            inventoryHandle = inventoryHandle,
                            operation = InventoryEventArgs.Operation.Withdrawn,
                        });
                    }
                }

                // Deposited
                if (changeevent.Value.IsValid()) {
                    itemOwners[changeevent.Value] = inventories[inventoryId];
                    if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                        inventoryCallbacks[inventoryHandle].Invoke(changeevent.PreviousValue, new InventoryEventArgs() {
                            itemHandle = changeevent.Value,
                            inventoryHandle = inventoryHandle,
                            operation = InventoryEventArgs.Operation.Deposited,
                        });
                    }
                }
            }
            else if (changeevent.Type is NetworkListEvent<ItemHandle>.EventType.Remove or NetworkListEvent<ItemHandle>.EventType.RemoveAt) {
                int inventoryId = Mathf.FloorToInt(changeevent.Index / InventorySlots);
                InventoryHandle inventoryHandle = inventories[inventoryId];
                itemOwners.Remove(changeevent.Value);

                if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                    inventoryCallbacks[inventoryHandle].Invoke(changeevent.PreviousValue, new InventoryEventArgs() {
                        itemHandle = changeevent.PreviousValue,
                        inventoryHandle = inventoryHandle,
                        operation = InventoryEventArgs.Operation.Deposited,
                    });
                }
            }
        }

        void InventoriesOnOnListChanged(NetworkListEvent<InventoryHandle> changeevent) {
            InvokeInventoryCreatedEvent(changeevent.Value);
        }

        void InvokeInventoryCreatedEvent(InventoryHandle inventoryHandle) {
            if (inventoryCallbacks.ContainsKey(inventoryHandle)) {
                inventoryCallbacks[inventoryHandle].Invoke(this, new InventoryEventArgs() {
                    itemHandle = default,
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

            if (HasAuthority == false) {
                AddInventoryRpc(inventoryHandle);
                return;
            }

            inventories.Add(inventoryHandle);
        }

        [Rpc(SendTo.Server)]
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

        public InventoryHandle GetInventoryHandle(ItemHandle itemHandle) {
            return itemOwners.GetValueOrDefault(itemHandle);
        }

        public List<ItemHandle> GetItems(InventoryHandle inventoryHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return null;

            List<ItemHandle> itemHandles = new(InventorySlots);
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                itemHandles.Add(inventoryItems[i]);
            }

            return itemHandles;
        }

        public IItemContainer GetItemContainer(InventoryHandle inventoryHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) return null;
            return itemContainers[inventoryHandle];
        }

        [Rpc(SendTo.Server)]
        void AddItemRpc(Item item) => AddItem(item);

        void AddItem(Item item) {
            if (HasAuthority == false) {
                AddItemRpc(item);
                return;
            }

            items.Add(item);
        }

        public ItemHandle CreateItem(string slug) {
            Item item = Item.Create(slug);
            AddItemRpc(item);
            return item.Handle;
        }

        public Item? GetItem(ItemHandle id) {
            foreach (Item item in items) {
                if (item.Handle == id) {
                    return item;
                }
            }

            return default;
        }

        [Rpc(SendTo.Server)]
        void PlaceItemInWorldRpc(ItemHandle itemHandle, Vector3 spawnPosition, Quaternion spawnRotation) => PlaceItemInWorld(itemHandle, spawnPosition, spawnRotation);

        public void PlaceItemInWorld(ItemHandle itemHandle, Vector3 spawnPosition, Quaternion spawnRotation) {
            if (IsServer == false) {
                PlaceItemInWorldRpc(itemHandle, spawnPosition, spawnRotation);
                return;
            }

            Item? item = GetItem(itemHandle);
            if (item.HasValue == false) {
                Debug.LogError("Trying to spawn null item");
                return;
            }

            if (IsInInventory(itemHandle)) {
                Withdraw(itemHandle);
            }

            GameManager.Spawner.SpawnItem(item.Value, spawnPosition, spawnRotation);
        }

        public void PickUpItem(ItemHandle itemAccessId) {
            Item? item = GetItem(itemAccessId);
            if (item.HasValue == false) {
                Debug.LogError("Trying to despawn null item");
                return;
            }

            // GameManager.Spawner.Despawn(item.Value);
        }

        public bool IsInInventory(ItemHandle itemHandle) {
            return itemOwners.ContainsKey(itemHandle);
        }

        bool ValidateInventoryHandle(InventoryHandle inventoryHandle) {
            if (inventoryHandle.IsValid() == false) {
                Debug.LogError($"Inventory handle not valid. ID {inventoryHandle.id}");
                return false;
            }

            return true;
        }

        [Rpc(SendTo.Server)]
        void DepositRpc(InventoryHandle inventoryHandle, ItemHandle itemHandle) => Deposit(inventoryHandle, itemHandle);

        public void Deposit(InventoryHandle inventoryHandle, ItemHandle itemHandle) {
            if (ValidateInventoryHandle(inventoryHandle) == false) {
                return;
            }

            if (HasAuthority == false) {
                DepositRpc(inventoryHandle, itemHandle);
                return;
            }

            if (IsInInventory(itemHandle)) {
                Withdraw(itemHandle);
            }

            bool successful = false;
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (inventoryItems[i].IsValid()) continue;
                inventoryItems[i] = itemHandle;
                successful = true;
                break;
            }

            if (successful == false) {
                Debug.LogError("Inventory Full");
            }
        }

        [Rpc(SendTo.Server)]
        void WithdrawRpc(ItemHandle itemHandle) => Withdraw(itemHandle);

        public void Withdraw(ItemHandle itemHandle) {
            if (HasAuthority == false) {
                WithdrawRpc(itemHandle);
                return;
            }

            InventoryHandle inventoryHandle = GetInventoryHandle(itemHandle);

            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (inventoryItems[i] != itemHandle) continue;
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