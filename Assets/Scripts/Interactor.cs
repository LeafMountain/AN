using System.Collections.Generic;
using Core;
using EventManager;
using InventorySystem;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Interactor : ActorComponent {
    public Actor Target { get; set; }

    [SerializeField] LayerMask interactLayerMask;

    public PlayerInventory Inventory;

    public void Interact() {
        if (Target == null) return;

        if (Target.TryGetComponent(out WorldItem storeable)) {
            GameManager.ItemManager.Deposit(Inventory.InventoryHandle, storeable.GetItemHandle());
            return;
        }

        IInteractable[] interactables = Target.GetComponents<IInteractable>();
        for (int i = 0; i < interactables.Length; i++) {
            interactables[i].Interact(Parent);
        }

        Events.TriggerEvent(Flag.LookTarget, Target);
    }

    public void Drop() {
        List<ItemHandle> items = GameManager.ItemManager.GetItems(Inventory.InventoryHandle);
        for (int i = items.Count - 1; i >= 0; i--) {
            if (items[i].IsValid() == false) continue;

            if (Target && Target.TryGetComponent(out WorldInventory worldInventory)) {
                GameManager.ItemManager.Deposit(worldInventory.InventoryHandle, items[i]);
                return;
            }

            Vector3 spawnPosition = transform.position + transform.forward * 2f;
            Quaternion spawnRotation = transform.rotation;
            GameManager.ItemManager.PlaceItemInWorld(items[i], spawnPosition, spawnRotation);
            
            break;
        }
    }

    public void ToggleInventory() {
        GameManager.UI.PlayerInventory.ToggleInventory();
    }

    public void Update() {
        Target = GetLookHit(Camera.main, interactLayerMask);
    }

    static Actor GetLookHit(Camera cam, LayerMask layerMask) {
        Transform camTransform = cam.transform;
        RaycastHit batchedHit = default;

        // raycast
        {
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

            Vector3 origin = camTransform.position;
            Vector3 direction = camTransform.forward;

            commands[0] = new RaycastCommand(origin, direction, new QueryParameters(layerMask));
            JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1);
            handle.Complete();
            batchedHit = results[0];

            results.Dispose();
            commands.Dispose();
        }

        // Sphere cast
        if (batchedHit.collider == null || batchedHit.collider.TryGetComponent(out Actor actor) == false) {
            var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            var commands = new NativeArray<SpherecastCommand>(1, Allocator.TempJob);

            Vector3 origin = camTransform.position;
            Vector3 direction = camTransform.forward;

            commands[0] = new SpherecastCommand(origin, 1f, direction, new QueryParameters(layerMask));
            JobHandle handle = SpherecastCommand.ScheduleBatch(commands, results, 1);
            handle.Complete();
            batchedHit = results[0];

            results.Dispose();
            commands.Dispose();
        }

        if (batchedHit.rigidbody != null && batchedHit.rigidbody.TryGetComponent(out actor)) {
            return actor;
        }
        else if (batchedHit.collider != null && batchedHit.collider.TryGetComponent(out actor)) {
            return actor;
        }

        return null;
    }
}