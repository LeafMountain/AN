using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class ItemManager : NetworkBehaviour
    {
        public readonly NetworkList<Item> Items = new();
        public readonly NetworkList<Inventory1> Inventories = new();
        
        public Dictionary<ItemAccessor, IItemContainer> itemContainers = new();

        public ItemAccessor CreateItem(string slug)
        {
            Item item = Item.Create(slug);
            Items.Add(item);
            return item.accessId;
        }

        public Item? GetItem(ItemAccessor id)
        {
            foreach (Item item in Items)
            {
                if (item.accessId == id)
                {
                    return item;
                }
            }

            return default;
        }

        public void PlaceItem(ItemAccessor itemAccessId, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            Item? item = GetItem(itemAccessId);
            if (item.HasValue == false)
            {
                Debug.LogError("Trying to spawn null item");
                return;
            }

            if (itemContainers.TryGetValue(itemAccessId, out IItemContainer itemContainer))
            {
                Withdraw(itemContainer, itemAccessId);
            }

            GameManager.Spawner.SpawnItem(item.Value, spawnPosition, spawnRotation);
        }

        public void PickUpItem(ItemAccessor itemAccessId)
        {
            Item? item = GetItem(itemAccessId);
            if (item.HasValue == false)
            {
                Debug.LogError("Trying to despawn null item");
                return;
            }

            // GameManager.Spawner.Despawn(item.Value);
        }

        public void Deposit(IItemContainer itemContainer, ItemAccessor itemAccessId)
        {
            if (itemContainers.TryGetValue(itemAccessId, out IItemContainer previousItemContainer))
            {
                Withdraw(previousItemContainer, itemAccessId);
            }
            
            itemContainer.DepositImplementation(itemAccessId);
            itemContainers[itemAccessId] = itemContainer;
        }

        public void Withdraw(IItemContainer itemContainer, ItemAccessor itemAccessId)
        {
            itemContainer.WithdrawImplementation(itemAccessId);     
            itemContainers.Remove(itemAccessId);
        }
    }

    public interface IItemContainer
    {
        public void DepositImplementation(ItemAccessor itemAccessId);
        public void WithdrawImplementation(ItemAccessor itemAccessId);
    }

    [Serializable, GenerateSerializationForType(typeof(Item))]
    public struct Item : IEquatable<Item>, INetworkSerializable
    {
        public ItemAccessor accessId;
        public int databaseId;

        public bool Equals(Item other) => accessId == other.accessId;
        public override bool Equals(object obj) => obj is Item other && Equals(other);
        public override int GetHashCode() => accessId.GetHashCode();

        public static Item Create(string slug)
        {
            return new Item
            {
                databaseId = GameManager.Database.StringToID(slug),
                accessId = ItemAccessor.Create(),
            };
        }

        public override string ToString()
        {
            return accessId.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref accessId);
            serializer.SerializeValue(ref databaseId);
        }
    }

    [Serializable, GenerateSerializationForType(typeof(Inventory1))]
    public struct Inventory1 : IEquatable<Inventory1>, INetworkSerializable
    {
        public int id;

        public override string ToString()
        {
            return id.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
        }

        public bool Equals(Inventory1 other) => id == other.id;
        public override bool Equals(object obj) => obj is Inventory1 other && Equals(other);
        public override int GetHashCode() => id;
    }
}