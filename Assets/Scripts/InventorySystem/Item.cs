using System;

namespace InventorySystem
{
    [Serializable]
    public struct Item : IEquatable<Item>
    {
        public ActorHandle Handle;
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
                Handle = ActorHandle.Create(),
            };
        }

        public override string ToString()
        {
            return Handle.ToString();
        }

        public bool IsValid() => Handle.IsValid();
    }
}