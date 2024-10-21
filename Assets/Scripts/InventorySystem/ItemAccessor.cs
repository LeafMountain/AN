using System;
using Unity.Netcode;

namespace InventorySystem
{
    public struct ItemAccessor : IEquatable<ItemAccessor>, INetworkSerializable
    {
        private int id;

        public static bool operator ==(ItemAccessor a, ItemAccessor b) => a.Equals(b);
        public static bool operator !=(ItemAccessor a, ItemAccessor b) => !a.Equals(b);
        public static ItemAccessor Empty { get; } = new();

        public bool Equals(ItemAccessor other) => id == other.id;
        public override bool Equals(object obj) => obj is ItemAccessor other && Equals(other);
        public override int GetHashCode() => id;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter => serializer.SerializeValue(ref id);

        public static ItemAccessor Create() => new() { id = UnityEngine.Random.Range(int.MinValue, int.MaxValue) };

        public bool IsValid() => id != 0;

        // public override string ToString()
        // {
        //     Item? item = GameManager.ItemManager.GetItem(this);
        //     return item.HasValue ? item.Value.ToString() : base.ToString();
        // }

        public Item? GetItem() => GameManager.ItemManager.GetItem(this);
    }
}