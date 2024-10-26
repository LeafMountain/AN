using Core;
using UnityEngine;

namespace InventorySystem {
    public class PlayerInventory : NetworkActorComponent, IItemContainer {
        public InventoryHandle InventoryHandle { get; set; }

        protected void Awake() {
            InventoryHandle = GameManager.ItemManager.CreateInventory();
            GameManager.ItemManager.AddCallback(InventoryHandle, OnInventoryCallback);
        }

        private void OnInventoryCallback(object sender, InventoryEventArgs args) {
            if (args.operation == InventoryEventArgs.Operation.Deposited) {
                Item? item = GameManager.ItemManager.GetItem(args.itemHandle);
                if (item.HasValue) {
                    ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                    Debug.Log($"Picked up {itemData.slug}");
                }
            }
            else if (args.operation == InventoryEventArgs.Operation.Withdrawn) {
                {
                    Item? item = GameManager.ItemManager.GetItem(args.itemHandle);
                    if (item.HasValue) {
                        ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                        Debug.Log($"Dropped up {itemData.slug}");
                    }
                }
            }
        }
    }
}