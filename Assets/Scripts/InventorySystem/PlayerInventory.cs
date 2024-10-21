using Core;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class PlayerInventory : NetworkActorComponent, IItemContainer
    {
        public readonly NetworkList<ItemAccessor> Items = new();

        protected void Awake()
        {
            Items.OnListChanged += ItemsOnOnListChanged;
        }

        private void ItemsOnOnListChanged(NetworkListEvent<ItemAccessor> changeevent)
        {
            switch (changeevent.Type)
            {
                case NetworkListEvent<ItemAccessor>.EventType.Add:
                {
                    Item? item = GameManager.ItemManager.GetItem(changeevent.Value);
                    if (item.HasValue)
                    {
                        ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                        Debug.Log($"Picked up {itemData.slug}");
                    }

                    break;
                }
                case NetworkListEvent<ItemAccessor>.EventType.Remove:
                case NetworkListEvent<ItemAccessor>.EventType.RemoveAt:
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

        public void DepositImplementation(ItemAccessor itemAccessId)
        {
            Items.Add(itemAccessId);
        }

        public void WithdrawImplementation(ItemAccessor itemAccessId)
        {
            Items.Remove(itemAccessId);
        }
    }
}