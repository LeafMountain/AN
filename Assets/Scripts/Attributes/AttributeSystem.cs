using System;
using System.Collections.Generic;
using EventManager;
using InventorySystem;
using Mirror;
using UnityEngine;

namespace Attributes
{
    public enum Attribute
    {
        None = 0,
        Health,
    }

    public readonly struct AttributeKey
    {
        public readonly ActorHandle handle;
        public readonly Attribute attribute;

        public AttributeKey(ActorHandle handle, Attribute attribute)
        {
            this.handle = handle;
            this.attribute = attribute;
        }
    }

    public class AttributeEventArgs : EventArgs
    {
        public Attribute attribute;
        public float newValue;
        public Vector3 position;
        public ActorHandle instigator;
    }

    public struct AttributeChange
    {
        public ActorHandle handle;
        public Attribute attribute;
        public Vector3 position;
        public ActorHandle instigator;
        public float valueDelta;
        public float maxDelta;
    }

    public class AttributeSystem : NetworkBehaviour
    {
        [ShowInInspector] readonly SyncDictionary<AttributeKey, float> current = new();
        [ShowInInspector] readonly SyncDictionary<AttributeKey, float> max = new();

        public SyncList<AttributeChange> attributeChanges = new();

        private void Start()
        {
            current.OnChange += OnCurrentUpdated;
            attributeChanges.OnChange += OnAttributeChanged;
        }

        private void OnAttributeChanged(SyncList<AttributeChange>.Operation operation, int i, AttributeChange change)
        {
            if (operation == SyncList<AttributeChange>.Operation.OP_REMOVEAT)
            {
                Events.TriggerEvent(Flag.AttributeUpdated, change.handle, new AttributeEventArgs()
                {
                    attribute = change.attribute,
                    newValue = GetCurrent(change.handle, change.attribute),
                    position = change.position,
                    instigator = change.instigator,
                });
            }
        }

        private void Update()
        {
            if (NetworkServer.active == false) return;
            for (var i = 0; attributeChanges.Count > 0 && i < 100; i++)
            {
                ApplyAttributeChange(attributeChanges[0]);
                attributeChanges.RemoveAt(0);
            }
        }

        private void ApplyAttributeChange(AttributeChange attributeChange)
        {
            if (attributeChange.maxDelta != 0)
            {
                // AddToCurrent(attributeChange.handle, attributeChange.attribute, attributeChange.valueDelta);        
            }

            if (attributeChange.valueDelta != 0)
            {
                AddToCurrent(attributeChange.handle, attributeChange.attribute, attributeChange.valueDelta);
            }
        }

        private void OnCurrentUpdated(SyncIDictionary<AttributeKey, float>.Operation op, AttributeKey key, float value)
        {
            switch (op)
            {
                case SyncIDictionary<AttributeKey, float>.Operation.OP_SET:
                {
                    break;
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdAddAttributeChange(AttributeChange attributeChange) => AddAttributeChange(attributeChange);

        public void AddAttributeChange(AttributeChange attributeChange)
        {
            if (NetworkServer.active == false)
            {
                CmdAddAttributeChange(attributeChange);
                return;
            }

            attributeChanges.Add(attributeChange);
        }

        public float GetCurrent(ActorHandle handle, Attribute attribute)
        {
            return current.GetValueOrDefault(new AttributeKey(handle, attribute));
        }

        public float GetMax(ActorHandle handle, Attribute attribute)
        {
            return max.GetValueOrDefault(new AttributeKey(handle, attribute));
        }

        public void AddToCurrent(ActorHandle handle, Attribute attribute, float value)
        {
            if (HasAttribute(handle, attribute) == false)
            {
                return;
            }

            SetCurrent(handle, attribute, GetCurrent(handle, attribute) + value);
        }

        public void SetCurrent(ActorHandle handle, Attribute attribute, float value)
        {
            if (HasAttribute(handle, attribute) == false)
            {
                return;
            }

            AttributeKey key = new(handle, attribute);
            current[key] = Mathf.Clamp(value, 0, GetMax(handle, attribute));
        }

        public bool HasAttribute(ActorHandle handle, Attribute attribute)
        {
            return current.ContainsKey(new AttributeKey(handle, attribute));
        }

        public void SetupAttribute(ActorHandle handle, Attribute attribute, int maxValue, int startValue = -1)
        {
            if (HasAttribute(handle, attribute)) return;
            if (startValue == -1) startValue = maxValue;
            max[new AttributeKey(handle, attribute)] = maxValue;
            current[new AttributeKey(handle, attribute)] = startValue;
        }

        [Command(requiresAuthority = false)]
        void CmdDoDamage(ActorHandle transmitterHandle, ActorHandle receiverHandle, Vector3 position) => DoDamage(transmitterHandle, receiverHandle, position);

        public void DoDamage(ActorHandle transmitterHandle, ActorHandle receiverHandle, Vector3 position)
        {
            if (NetworkServer.active == false)
            {
                CmdDoDamage(transmitterHandle, receiverHandle, position);
                return;
            }

            // AddToCurrent(receiverHandle, Attribute.Health, -1);
            AddAttributeChange(new AttributeChange()
            {
                attribute = Attribute.Health,
                handle = receiverHandle,
                instigator = transmitterHandle,
                valueDelta = -1,
                position = position,
            });
        }
    }
}