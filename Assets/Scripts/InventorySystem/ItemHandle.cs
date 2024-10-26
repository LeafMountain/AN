using System;
using Unity.Netcode;

namespace InventorySystem {
    public struct ItemHandle : IEquatable<ItemHandle>, INetworkSerializable {
        public int id;

        public static bool operator ==(ItemHandle a, ItemHandle b) => a.Equals(b);
        public static bool operator !=(ItemHandle a, ItemHandle b) => !a.Equals(b);

        public bool Equals(ItemHandle other) => id == other.id;
        public override bool Equals(object obj) => obj is ItemHandle other && Equals(other);
        public override int GetHashCode() => id;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter => serializer.SerializeValue(ref id);

        public static ItemHandle Create() => new() { id = UnityEngine.Random.Range(int.MinValue, int.MaxValue) };

        public bool IsValid() => id != 0;

        public override string ToString() {
            if (id == 0) return 0.ToString();
            Item? item = GameManager.ItemManager.GetItem(this);
            return item.HasValue ? item.Value.ToString() : base.ToString();
        }

        public Item? GetItem() => GameManager.ItemManager.GetItem(this);
    }
}