using System;

namespace InventorySystem {
    public class InventoryEventArgs : EventArgs {
        public enum Operation {
            None,
            Withdrawn,
            Deposited,
            InventoryCreated,
        }
        
        public ActorHandle ActorHandle;
        public Operation operation;
        public InventoryHandle inventoryHandle;
    }
}