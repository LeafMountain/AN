using System.Collections.Generic;
using System.Linq;
using Core;
using Mirror;
using UnityEngine;

namespace InventorySystem {
    public class PlayerInventory : NetworkBehaviour, IItemContainer {
        public InventoryHandle InventoryHandle { 
            get => inventoryHandle;
            set => inventoryHandle = value;
        }

        [SyncVar] InventoryHandle inventoryHandle = new();

        public List<Item> debugItems = new();

        public override void OnStartClient() {
            if (NetworkServer.active) {
                InventoryHandle = GameManager.ItemManager.CreateInventory();
            }
            
            GameManager.ItemManager.AddCallback(InventoryHandle, OnInventoryCallback);
        }

        void OnInventoryCallback(object sender, InventoryEventArgs args) {
            if (args.operation == InventoryEventArgs.Operation.Deposited) {
                Item? item = GameManager.ItemManager.GetItem(args.ActorHandle);
                if (item.HasValue) {
                    ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                    Debug.Log($"Picked up {itemData.slug}");
                }
            }
            else if (args.operation == InventoryEventArgs.Operation.Withdrawn) {
                {
                    Item? item = GameManager.ItemManager.GetItem(args.ActorHandle);
                    if (item.HasValue) {
                        ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                        Debug.Log($"Dropped up {itemData.slug}");
                    }
                }
            }
        }

        void OnDrawGizmosSelected() {
            if (Application.isPlaying == false) return;
            debugItems = GameManager.ItemManager.GetItems(InventoryHandle)
                .Where(x => x.IsValid())
                .Select(x => GameManager.ItemManager.GetItem(x).Value)
                .ToList();
        }
    }
}