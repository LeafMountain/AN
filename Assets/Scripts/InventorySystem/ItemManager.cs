using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem {
    public class ItemManager : NetworkBehaviour {
        const int InventorySlots = 16;

        readonly NetworkList<Item> Items = new();
        readonly NetworkList<ItemHandle> InventoryItems = new();
        readonly NetworkList<InventoryHandle> Inventories = new();
        readonly NetworkList<InventoryHandle> FreeInventories = new(); // Might only exist on server

        readonly Dictionary<ItemHandle, InventoryHandle> itemsInInventory = new();
        readonly Dictionary<InventoryHandle, IItemContainer> itemContainers = new();
        readonly Dictionary<InventoryHandle, EventHandler<InventoryEventArgs>> InventoryCallbacks = new();

        public InventoryHandle CreateInventory() {
            if (FreeInventories.Count > 0) {
                InventoryHandle freeInventory = FreeInventories[0];
                FreeInventories.RemoveAt(0);
                return freeInventory;
            }

            InventoryHandle inventory = new();

            for (int i = 0; i < InventorySlots; i++) {
                InventoryItems.Add(default);
            }

            inventory.id = InventoryItems.Count / InventorySlots;
            Inventories.Add(inventory);
            return inventory;
        }

        void AddInventory(InventoryHandle inventoryHandle) {
            if (HasAuthority == false) {
                AddInventoryRpc(inventoryHandle);
                return;
            }

            Inventories.Add(inventoryHandle);
        }

        [Rpc(SendTo.Server)]
        void AddInventoryRpc(InventoryHandle inventoryHandle) => AddInventory(inventoryHandle);

        public void ReturnInventory(InventoryHandle handle) {
            FreeInventories.Add(handle);
        }

        public InventoryHandle GetInventoryHandle(int id) {
            if (Inventories.Count < id) {
                Debug.LogError("Inventory outside of range");
                return InventoryHandle.Empty;
            }

            return Inventories[id];
        }

        public InventoryHandle GetInventoryHandle(ItemHandle itemHandle) {
            return itemsInInventory.GetValueOrDefault(itemHandle);
        }

        public List<ItemHandle> GetItems(InventoryHandle inventoryHandle) {
            List<ItemHandle> itemHandles = new();
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                itemHandles.Add(InventoryItems[i]);
            }

            return itemHandles;
        }

        public IItemContainer GetItemContainer(InventoryHandle inventoryHandle) {
            return itemContainers[inventoryHandle];
        }

        [Rpc(SendTo.Server)]
        void AddItemRpc(Item item) => AddItem(item);

        void AddItem(Item item) {
            if (HasAuthority == false) {
                AddItemRpc(item);
                return;
            }

            Items.Add(item);
        }

        public ItemHandle CreateItem(string slug) {
            Item item = Item.Create(slug);
            AddItemRpc(item);
            return item.Handle;
        }

        public Item? GetItem(ItemHandle id) {
            foreach (Item item in Items) {
                if (item.Handle == id) {
                    return item;
                }
            }

            return default;
        }

        [Rpc(SendTo.Server)]
        void PlaceItemInWorldRpc(ItemHandle itemHandle, Vector3 spawnPosition, Quaternion spawnRotation) => PlaceItemInWorld(itemHandle, spawnPosition, spawnRotation);

        public void PlaceItemInWorld(ItemHandle itemHandle, Vector3 spawnPosition, Quaternion spawnRotation) {
            if (HasAuthority == false) {
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
            return itemsInInventory.ContainsKey(itemHandle);
        }

        public void Deposit(InventoryHandle inventoryHandle, ItemHandle itemHandle) {
            if (IsInInventory(itemHandle)) {
                Withdraw(itemHandle);
            }

            // Deposit here
            bool successful = false;
            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (this.InventoryItems[i].IsValid()) continue;
                this.InventoryItems[i] = itemHandle;
                itemsInInventory.Add(itemHandle, inventoryHandle);
                successful = true;
                break;
            }

            if (successful == false) {
                Debug.LogError("Inventory Full");
            }

            if (InventoryCallbacks.ContainsKey(inventoryHandle)) {
                InventoryCallbacks[inventoryHandle].Invoke(itemHandle, new InventoryEventArgs() {
                    itemHandle = itemHandle,
                    inventoryHandle = inventoryHandle,
                    operation = InventoryEventArgs.Operation.Deposited,
                });
            }
        }

        public void Withdraw(ItemHandle itemHandle) {
            InventoryHandle inventoryHandle = GetInventoryHandle(itemHandle);
            itemsInInventory.Remove(itemHandle);

            for (int i = (inventoryHandle.id - 1) * InventorySlots; i < (inventoryHandle.id - 1) * InventorySlots + InventorySlots; i++) {
                if (this.InventoryItems[i] != itemHandle) continue;
                itemsInInventory.Remove(itemHandle);
                InventoryItems[i] = default;
                break;
            }

            if (InventoryCallbacks.ContainsKey(inventoryHandle)) {
                InventoryCallbacks[inventoryHandle].Invoke(itemHandle, new InventoryEventArgs() {
                    itemHandle = itemHandle,
                    inventoryHandle = inventoryHandle,
                    operation = InventoryEventArgs.Operation.Withdrawn,
                });
            }
        }

        public void AddCallback(InventoryHandle inventoryHandle, EventHandler<InventoryEventArgs> callback) {
            InventoryCallbacks.TryAdd(inventoryHandle, default);
            InventoryCallbacks[inventoryHandle] += callback;
        }

        public void RemoveCallback(InventoryHandle inventoryHandle, EventHandler<InventoryEventArgs> callback) {
            if (InventoryCallbacks.ContainsKey(inventoryHandle)) {
                InventoryCallbacks[inventoryHandle] -= callback;
                if (InventoryCallbacks[inventoryHandle] == null) {
                    InventoryCallbacks.Remove(inventoryHandle);
                }
            }
        }
    }
}