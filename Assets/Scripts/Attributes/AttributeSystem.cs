using System.Collections.Generic;
using InventorySystem;
using Mirror;
using UnityEngine;

namespace Attributes {
    public enum Attribute {
        None = 0,
        Health,
    }

    public readonly struct AttributeKey {
        public readonly ActorHandle handle;
        public readonly Attribute attribute;

        public AttributeKey(ActorHandle handle, Attribute attribute) {
            this.handle = handle;
            this.attribute = attribute;
        }
    }

    public class AttributeSystem : NetworkBehaviour {
        [ShowInInspector] readonly SyncDictionary<AttributeKey, float> current = new();
        [ShowInInspector] readonly SyncDictionary<AttributeKey, float> max = new();
        
        public float GetCurrent(ActorHandle handle, Attribute attribute) {
            return current.GetValueOrDefault(new AttributeKey(handle, attribute));
        }

        public float GetMax(ActorHandle handle, Attribute attribute) {
            return max.GetValueOrDefault(new AttributeKey(handle, attribute));
        }

        public void AddToCurrent(ActorHandle handle, Attribute attribute, float value) {
            if (HasAttribute(handle, attribute) == false) {
                return;
            }
            
            SetCurrent(handle, attribute, GetCurrent(handle, attribute) + value);
        }

        public void SetCurrent(ActorHandle handle, Attribute attribute, float value) {
            if (HasAttribute(handle, attribute) == false) {
                return;
            }
            
            AttributeKey key = new(handle, attribute);
            current[key] = Mathf.Clamp(value, 0, GetMax(handle, attribute));
        }

        public bool HasAttribute(ActorHandle handle, Attribute attribute) {
            return current.ContainsKey(new AttributeKey(handle, attribute));
        }

        [Command(requiresAuthority = false)]
        void CmdDoDamage(ActorHandle transmitterHandle, ActorHandle receiverHandle) => DoDamage(transmitterHandle, receiverHandle);
        public void DoDamage(ActorHandle transmitterHandle, ActorHandle receiverHandle) {

            if (NetworkServer.active == false) {
                CmdDoDamage(transmitterHandle, receiverHandle);
                return;
            }
            
            AddToCurrent(receiverHandle, Attribute.Health, -1);
        }
    }
}