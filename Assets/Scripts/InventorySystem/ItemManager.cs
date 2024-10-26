using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem {
    public class ItemManager : NetworkBehaviour {
        private const int InventorySlots = 16;

        private readonly NetworkList<Item> Items = new();
        private readonly NetworkList<ItemHandle> InventoryItems = new();
        private readonly NetworkList<InventoryHandle> Inventories = new();

        private readonly Dictionary<ItemHandle, InventoryHandle> itemsInInventory = new();
        private readonly Dictionary<InventoryHandle, IItemContainer> itemContainers = new();
        private readonly Dictionary<InventoryHandle, EventHandler<InventoryEventArgs>> InventoryCallbacks = new();

        public InventoryHandle CreateInventory() {
            InventoryHandle inventory = new();

            for (int i = 0; i < InventorySlots; i++) {
                InventoryItems.Add(ItemHandle.Empty);
            }

            inventory.id = InventoryItems.Count / InventorySlots;
            Inventories.Add(inventory);
            return inventory;
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

        public List<Item> GetItems(InventoryHandle inventoryHandle) {
            List<Item> items = new();
            for (var i = inventoryHandle.id; i < inventoryHandle.id + InventorySlots; i++) {
                items.Add(Items[i]);
            }

            return items;
        }

        public IItemContainer GetItemContainer(InventoryHandle inventoryHandle) {
            return itemContainers[inventoryHandle];
        }

        public ItemHandle CreateItem(string slug) {
            Item item = Item.Create(slug);
            Items.Add(item);
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

        public void PlaceItemInWorld(ItemHandle itemHandle, Vector3 spawnPosition, Quaternion spawnRotation) {
            Item? item = GetItem(itemHandle);
            if (item.HasValue == false) {
                Debug.LogError("Trying to spawn null item");
                return;
            }

            if (IsInInventory(itemHandle)){
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
            for (int i = inventoryHandle.id; i < inventoryHandle.id + InventorySlots; i++) {
                if (this.InventoryItems[i].IsValid()) continue;
                this.InventoryItems[i] = itemHandle;
                itemsInInventory.Add(itemHandle, inventoryHandle);
                break;
            }

            if (InventoryCallbacks.TryGetValue(inventoryHandle, out EventHandler<InventoryEventArgs> eventHandler)) {
                eventHandler.Invoke(itemHandle, new InventoryEventArgs() {
                    itemHandle = itemHandle,
                    inventoryHandle = inventoryHandle,
                    operation = InventoryEventArgs.Operation.Deposited,
                });
            }
        }

        public void Withdraw(ItemHandle itemHandle) {
            InventoryHandle inventoryHandle = GetInventoryHandle(itemHandle);
            itemsInInventory.Remove(itemHandle);

            for (int i = inventoryHandle.id; i < inventoryHandle.id + InventorySlots; i++) {
                if (this.InventoryItems[i] != itemHandle) continue;
                itemsInInventory.Remove(itemHandle);
                InventoryItems[i] = ItemHandle.Empty;
                break;
            }

            // IItemContainer itemContainer = GetItemContainer(inventory);
            // itemContainer.WithdrawImplementation(itemAccessId);
            // itemContainers.Remove(itemAccessId);

            if (InventoryCallbacks.TryGetValue(inventoryHandle, out EventHandler<InventoryEventArgs> eventHandler)) {
                eventHandler.Invoke(itemHandle, new InventoryEventArgs() {
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
    }
}