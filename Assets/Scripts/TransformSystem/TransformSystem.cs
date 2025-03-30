using System.Collections.Generic;
using InventorySystem;
using Mirror;
using UnityEngine;

namespace TransformSystem
{
    public class TransformSystem
    {
        public SyncDictionary<ActorHandle, Vector3> positions = new();
        public SyncDictionary<ActorHandle, Vector3> rotations = new();

        public void SetPosition(ActorHandle handle, Vector3 position) => positions[handle] = position;
        public Vector3 GetPosition(ActorHandle handle) => positions.GetValueOrDefault(handle);
        public Vector3 GetRotation(ActorHandle handle) => rotations.GetValueOrDefault(handle);
    }
}