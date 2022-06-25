using System;
using System.Collections;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using EventManager;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public int supplies = 0;
    public int maxEnergy = 100;
    public int energy = 100;
    public float lootRange = 2f;
    public float lootSpeed = 5f;

    private Collider[] colliders = new Collider[10];
    public LayerMask actorMask;
    private Tweener lootTween;

    private bool dead = false;

    public RaycastHit MouseHit;
    public GameObject debugMousePositionObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var player = ANNetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            GameManager.Instance.localPlayer = player.GetComponent<Player>();
            var controller = player.GetComponent<ThirdPersonController>();
            var input = player.GetComponent<PlayerInput>();
            var cameraFollower = FindObjectOfType<CinemachineVirtualCamera>();
            cameraFollower.Follow = controller.CinemachineCameraTarget.transform;

            controller.enabled = true;
            input.enabled = true;
        }

        StartCoroutine(EnergyTick());

        Events.AddListener(Flag.DamageRecieved, this, OnDamageReveived);
    }

    private async void OnDamageReveived(object origin, EventArgs eventargs)
    {
        if (dead) return;
        var damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed)
        {
            dead = true;
            await Task.Delay(5000);
            Reset();
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = Vector3.zero;
    }

    public IEnumerator EnergyTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            energy -= 1;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (IsServer)
        {
            LootMagnet();
        }
    }

    protected override void Update()
    {
        base.Update();
        
        UpdateMouseHit();
        if (debugMousePositionObject)
        {
            debugMousePositionObject.transform.position = MouseHit.point;
            debugMousePositionObject.transform.forward = MouseHit.normal;
        }
    }

    private void LootMagnet()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, lootRange, colliders, actorMask);
        for (int i = 0; i < hits; i++)
        {
            var actor = colliders[i].GetComponent<Actor>();
            var storeable = actor.GetComponent<Storeable>();
            float distance = Vector3.Distance(transform.position + Vector3.up, actor.transform.position);
            if (distance < 1f)
            {
                NetworkObject networkObject = storeable.GetComponent<NetworkObject>();
                storeable.PickUp(this);
                // networkObject.Despawn();
            }
            else
            {
                actor.GetComponent<Rigidbody>().isKinematic = true;
                storeable.transform.position += (transform.position + Vector3.up - actor.transform.position).normalized * lootSpeed * Time.fixedDeltaTime;
            }
        }
    }

    public void AddEnergy(int value)
    {
        energy += value;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    protected override void Reset()
    {
        base.Reset();

        damageReceiver.Reset();
        supplies = 0;
        energy = 100;
        dead = false;
    }

    private void UpdateMouseHit()
    {
        // Vector2 mousePosition = Mouse.current.position.ReadValue();
        var mousePosition = Input.mousePosition;
        Ray cameraRay = GameManager.Instance.characterCamera.ScreenPointToRay(mousePosition);
        Physics.Raycast(cameraRay, out MouseHit);
    }
}