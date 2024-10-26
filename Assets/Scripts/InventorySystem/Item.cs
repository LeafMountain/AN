using System;
using Unity.Netcode;

namespace InventorySystem
{
    [Serializable, GenerateSerializationForType(typeof(Item))]
    public struct Item : IEquatable<Item>, INetworkSerializable
    {
        public ItemHandle Handle;
        public int databaseId;
        public static Item Empty { get; set; }

        public bool Equals(Item other) => Handle == other.Handle;
        public override bool Equals(object obj) => obj is Item other && Equals(other);
        public override int GetHashCode() => Handle.GetHashCode();

        public static Item Create(string slug)
        {
            return new Item
            {
                databaseId = GameManager.Database.StringToID(slug),
                Handle = ItemHandle.Create(),
            };
        }

        public override string ToString()
        {
            return Handle.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Handle);
            serializer.SerializeValue(ref databaseId);
        }
        
        public bool IsValid() => Handle.IsValid();
    }
}