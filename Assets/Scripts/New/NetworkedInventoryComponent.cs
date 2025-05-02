using System.Collections.Generic;
using Fusion;

public class NetworkInventoryComponent : NetworkBehaviour
{
    // [SyncVar] private List<InventoryItem> syncedInventory;
    //
    // [Command]
    // public void CmdAddItem(string itemID, int amount)
    // {
    //     // Add the item to the inventory and sync it
    //     AddItem(itemID, amount);
    //     RpcUpdateInventory();
    // }
    //
    // [ClientRpc]
    // private void RpcUpdateInventory()
    // {
    //     // Update inventory on all clients (this would update syncedInventory)
    // }
}