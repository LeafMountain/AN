using System;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour, IInteractable
    {
        public struct ItemData : IEquatable<ItemData>
        {
            public int itemID;

            public bool Equals(ItemData other) => itemID == other.itemID;
            public override bool Equals(object obj) => obj is ItemData other && Equals(other);
            public override int GetHashCode() => itemID;
        }
    
        public NetworkVariable<int> currentHealth;
        // public NetworkList<ItemData> items = new();

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