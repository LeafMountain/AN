using System;

namespace InventorySystem {
    public class InventoryEventArgs : EventArgs {
        public enum Operation {
            None,
            Withdrawn,
            Deposited,
            InventoryCreated,
        }
        
        public ItemHandle itemHandle;
        public Operation operation;
        public InventoryHandle inventoryHandle;
    }
}