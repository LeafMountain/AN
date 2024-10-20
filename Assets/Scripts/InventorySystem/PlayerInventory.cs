using Core;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class PlayerInventory : NetworkActorComponent, IItemContainer
    {
        public readonly NetworkList<int> Items = new();

        protected void Awake()
        {
            Items.OnListChanged += ItemsOnOnListChanged;
        }

        private void ItemsOnOnListChanged(NetworkListEvent<int> changeevent)
        {
            switch (changeevent.Type)
            {
                case NetworkListEvent<int>.EventType.Add:
                {
                    Item? item = GameManager.ItemManager.GetItem(changeevent.Value);
                    if (item.HasValue)
                    {
                        ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                        Debug.Log($"Picked up {itemData.slug}");
                    }

                    break;
                }
                case NetworkListEvent<int>.EventType.Remove:
                case NetworkListEvent<int>.EventType.RemoveAt:
                {
                    Item? item = GameManager.ItemManager.GetItem(changeevent.PreviousValue);
                    if (item.HasValue)
                    {
                        ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                        Debug.Log($"Dropped up {itemData.slug}");
                    }

                    break;
                }
            }
        }

        public void DepositImplementation(int itemAccessId)
        {
            Items.Add(itemAccessId);
        }

        public void WithdrawImplementation(int itemAccessId)
        {
            Items.Remove(itemAccessId);
        }
    }
}