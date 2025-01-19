using System;
using System.Collections.Generic;
using EventManager;
using InventorySystem;
using Mirror;

public struct EquipmentKey {
    public ActorHandle handle;
    public int slot;
}

public class ActiveItemEventArgs : EventArgs {
    public enum Operation {
        Added,
        Removed
    }

    public Operation operation;
    public ActorHandle itemHandle;
}

public class EquipmentSystem : NetworkBehaviour {
    [ShowInInspector] readonly SyncDictionary<ActorHandle, ActorHandle> activeItem = new();
    [ShowInInspector] readonly SyncDictionary<EquipmentKey, ActorHandle> equipment = new();

    public override void OnStartServer() {
        activeItem.OnChange += OnActiveItemChanged;
    }

    public override void OnStopServer() {
        activeItem.OnChange -= OnActiveItemChanged;
    }

    void OnActiveItemChanged(SyncIDictionary<ActorHandle, ActorHandle>.Operation op, ActorHandle ownerHandle, ActorHandle itemHandle) {
        switch (op) {
            case SyncIDictionary<ActorHandle, ActorHandle>.Operation.OP_ADD:
            case SyncIDictionary<ActorHandle, ActorHandle>.Operation.OP_SET: {
                Events.TriggerEvent(Flag.ActiveItemUpdated, ownerHandle, new ActiveItemEventArgs() {
                    operation = ActiveItemEventArgs.Operation.Added,
                    itemHandle = itemHandle,
                });
                break;
            }
            case SyncIDictionary<ActorHandle, ActorHandle>.Operation.OP_REMOVE: {
                Events.TriggerEvent(Flag.ActiveItemUpdated, ownerHandle, new ActiveItemEventArgs() {
                    operation = ActiveItemEventArgs.Operation.Removed,
                    itemHandle = itemHandle,
                });
                break;
            }
        }
    }

    public void SetActiveItem(ActorHandle ownerHandle, ActorHandle item) {
        activeItem[ownerHandle] = item;
    }

    public void ClearActiveItem(ActorHandle ownerHandle) {
        activeItem.Remove(ownerHandle);
    }

    public ActorHandle GetActiveItem(ActorHandle actorHandle) {
        return activeItem.GetValueOrDefault(actorHandle);
    }
}