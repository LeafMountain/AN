using System;
using Unity.Netcode;

namespace InventorySystem
{
    [Serializable, GenerateSerializationForType(typeof(InventoryHandle))]
    public struct InventoryHandle : IEquatable<InventoryHandle>, INetworkSerializable
    {
        public int id;
        public static InventoryHandle Empty { get; set; }

        public override string ToString()
        {
            return id.ToString();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
        }

        public bool Equals(InventoryHandle other) => id == other.id;
        public override bool Equals(object obj) => obj is InventoryHandle other && Equals(other);
        public override int GetHashCode() => id;
        
    }
}