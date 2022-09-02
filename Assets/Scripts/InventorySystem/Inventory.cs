using System;
using Core;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : NetworkActorComponent, IInteractable
    {
        public struct ItemData : IEquatable<ItemData>
        {
            public int itemID;

            public bool Equals(ItemData other) => itemID == other.itemID;
            public override bool Equals(object obj) => obj is ItemData other && Equals(other);
            public override int GetHashCode() => itemID;
        }
    
        public NetworkVariable<int> currentHealth;

        private NetworkList<ulong> itemNetIDs;
        public Storeable[] items = new Storeable[32];
        public int size;

        private void Awake()
        {
            itemNetIDs ??= new NetworkList<ulong>();
        }

        protected override void Start()
        {
            itemNetIDs.OnListChanged += ItemNetIDsOnOnListChanged;
        }

        private void ItemNetIDsOnOnListChanged(NetworkListEvent<ulong> changeevent)
        {
            switch (changeevent.Type)
            {
                case NetworkListEvent<ulong>.EventType.Add:
                    items[changeevent.Index] = NetworkManager.Singleton.SpawnManager.SpawnedObjects[changeevent.Value].GetComponent<Storeable>();
                    break;
                case NetworkListEvent<ulong>.EventType.Insert:
                    break;
                case NetworkListEvent<ulong>.EventType.Remove:
                    break;
                case NetworkListEvent<ulong>.EventType.RemoveAt:
                    break;
                case NetworkListEvent<ulong>.EventType.Value:
                    break;
                case NetworkListEvent<ulong>.EventType.Clear:
                    break;
                case NetworkListEvent<ulong>.EventType.Full:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Deposit(Storeable gameObject)
        {
             
        }

        public void Interact(Actor interactor)
        {
            Debug.Log("Open Inventory");
        }

        public string GetPrompt()
        {
            return "Open Inventory";
        }
    }
}