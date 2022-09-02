using Core;
using EventManager;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Interactor : ActorComponent
{
    private Actor target;
    public Actor Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField] private LayerMask interactLayerMask;

    public void Interact()
    {
        if(target == null) return;
        
        var interactables = target.GetComponents<IInteractable>();
        for (var i = 0; i < interactables.Length; i++)
        {
            interactables[i].Interact(Parent);
        }
        
        Events.TriggerEvent(Flag.LookTarget, target);
    }

    public void Update()
    {
        Target = GetLookHit(Camera.main, interactLayerMask);
    }

    private static Actor GetLookHit(Camera cam, LayerMask layerMask)
    {
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
        if (batchedHit.collider == null || batchedHit.collider.TryGetComponent(out Actor actor) == false)
        {
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

        if (batchedHit.collider == null || batchedHit.collider.TryGetComponent(out actor) == false)
        {
            return null;
        }

        return actor;
    }
}