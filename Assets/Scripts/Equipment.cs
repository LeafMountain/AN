using System;
using Core;
using EventManager;
using InventorySystem;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class Equipment : NetworkActorComponent {
    [Header("Settings")] public Transform weaponAttach;
    float weaponPushback = 0f;

    [Header("Runtime")] public Gun weapon;
    public Vector3 weaponOffset = Vector3.forward;

    public override void OnStartClient() {
        Events.AddListener(Flag.ActiveItemUpdated, Parent.handle, OnActiveItemUpdated);
        ActorHandle itemHandle = GameManager.Instance.CreateActor("test_item");
        handle.SetActiveItem(itemHandle);
    }

    void OnActiveItemUpdated(object origin, EventArgs eventargs) {
        ActiveItemEventArgs args = eventargs as ActiveItemEventArgs;

        switch (args.operation) {
            case ActiveItemEventArgs.Operation.Added: {
                args.itemHandle.Spawn(() => {
                    weapon = args.itemHandle.Value.GetComponent<Gun>();
                    weapon.transform.SetParent(weaponAttach);
                    weapon.transform.localPosition = default;
                    weapon.transform.localRotation = default;
                    foreach (Collider collider in weapon.GetComponentsInChildren<Collider>()) {
                        collider.enabled = false;
                    }
                });
                break;
            }
            case ActiveItemEventArgs.Operation.Removed: {
                break;
            }
        }
    }

    public void UseWeapon() {
        if(weapon == null) return;
        weapon.Fire();
        AddWeaponPushback();
    }

    void UpdateWeaponPosition() {
        weaponPushback -= Time.deltaTime * 2f;
        weaponPushback = math.clamp(weaponPushback, 0, float.MaxValue);

        var pushback = weaponAttach.TransformVector(Vector3.back) * weaponPushback;

        weaponAttach.forward = GameManager.CameraController.camera.transform.forward;
        // weaponAttach.position = transform.position + CameraController.Instance.camera.transform.forward + transform.TransformVector(weaponOffset) + pushback;
    }

    public override void OnStartServer() {
        SetSlot(0);

        GameObject spawned = GameManager.Spawner.Spawn(GameManager.Instance.defaultGun);
        spawned.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

        // spawned.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        EquipGun_ClientRpc(spawned.GetComponent<Gun>());
    }

    void LateUpdate() {
        if (weapon == null) return;

        UpdateWeaponPosition();
        // weapon.transform.position = weaponAttach.transform.position;
        weapon.transform.rotation = weaponAttach.transform.rotation;
    }

    [ClientRpc]
    public void EquipGun_ClientRpc(Gun gun) {
        // if (weapon.TryGet(out Gun gun)) {
        weapon = gun;
        OnGunEquipped(null, gun);
        // }
    }

    public void OnGunEquipped(Gun oldValue, Gun newValue) {
        if (oldValue) {
            foreach (Collider collider in oldValue.GetComponentsInChildren<Collider>()) {
                collider.enabled = true;
            }
        }

        if (newValue == null) {
            return;
        }

        newValue.transform.parent = transform;
        newValue.transform.position = weaponAttach.transform.position;
        newValue.transform.rotation = weaponAttach.transform.rotation;
        foreach (Collider collider in newValue.GetComponentsInChildren<Collider>()) {
            collider.enabled = false;
        }
    }

    public void OnDestroy() {
        if (weapon == null) return;
        if (Application.isPlaying == false) return;
        // weapon.GetComponent<NetworkObject>().Despawn();
        NetworkServer.Destroy(weapon.gameObject);
    }

    public float maxPushback = .2f;

    public void AddWeaponPushback() {
        weaponPushback = maxPushback;
    }

    public bool HasWeapon() {
        return weapon;
    }

    public void AimAt(Vector3 lookAtPoint) {
        weapon.transform.LookAt(lookAtPoint);
        var gunRotation = weapon.transform.rotation.eulerAngles;
        if (gunRotation.x < 200f) gunRotation.x += 360f;
        // gunRotation.x = Mathf.Clamp(gunRotation.x, 300f, 370f);
        weapon.transform.rotation = Quaternion.Euler(gunRotation);
    }

    public void Aim(Vector3 aimPosition) {
        weapon.Aim(aimPosition);
    }

    public void StopAim() {
        weapon?.StopAim();
    }

    public void SetSlot(int index) {
        if (weapon) {
            NetworkServer.Destroy(weapon.gameObject);
        }

        Gun spawned = GameManager.Spawner.Spawn(GameManager.Instance.guns[index]);
        spawned.netIdentity.AssignClientAuthority(connectionToClient);

        // spawned.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        // EquipGun_ClientRpc(spawned.GetComponent<Gun>());
    }
}