using System;

namespace InventorySystem
{
    [Serializable, Obsolete]
    public struct InventoryHandle : IEquatable<InventoryHandle>
    {
        public int id;
        public static InventoryHandle Empty { get; set; }

        public override string ToString()
        {
            return id.ToString();
        }

        public bool Equals(InventoryHandle other) => id == other.id;
        public override bool Equals(object obj) => obj is InventoryHandle other && Equals(other);
        public override int GetHashCode() => id;

        public bool IsValid() => id != 0;
    }
}