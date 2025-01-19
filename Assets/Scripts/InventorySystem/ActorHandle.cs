using System;
using Core;
using Newtonsoft.Json;

namespace InventorySystem {
    public struct ActorHandle : IEquatable<ActorHandle> {
        public Guid id;

        public ActorHandle(Guid id) => this.id = id;
        public static bool operator ==(ActorHandle a, ActorHandle b) => a.Equals(b);
        public static bool operator !=(ActorHandle a, ActorHandle b) => !a.Equals(b);

        public bool Equals(ActorHandle other) => id == other.id;
        public override bool Equals(object obj) => obj is ActorHandle other && Equals(other);
        public override int GetHashCode() => id.GetHashCode();

        public static ActorHandle Create() => new(Guid.NewGuid());

        public bool IsValid() => id != Guid.Empty;

        public override string ToString() {
            if (IsValid()) return 0.ToString();
            Item? item = GameManager.ItemManager.GetItem(this);
            return item.HasValue ? item.Value.ToString() : base.ToString();
        }

        public Item? GetItem() => GameManager.ItemManager.GetItem(this);

        [JsonIgnore] public string Data => GameManager.GetData(this);
        [JsonIgnore] public Actor Value => GameManager.GetSpawned(this);
        [JsonIgnore] public ActorHandle ActiveItem => GameManager.Equipment.GetActiveItem(this);
        public void SetActiveItem(ActorHandle handle) => GameManager.Equipment.SetActiveItem(this, handle);

        public void Spawn(Action onCompleteCallback = default) => GameManager.Spawn(this, onCompleteCallback);
        public void Despawn() => GameManager.Despawn(this);
    }
}