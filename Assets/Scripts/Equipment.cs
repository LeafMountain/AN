using Core;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Equipment : NetworkActorComponent
{
    [Header("Settings")] public Transform weaponAttach;
    private float weaponPushback = 0f;

    [Header("Runtime")] public Gun weapon;

    public void UseWeapon()
    {
        weapon.Fire();
        AddWeaponPushback();
    }

    public Vector3 weaponOffset = Vector3.forward;

    private void UpdateWeaponPosition()
    {
        weaponPushback -= Time.deltaTime * 2f;
        weaponPushback = math.clamp(weaponPushback, 0, float.MaxValue);

        var pushback = weaponAttach.TransformVector(Vector3.back) * weaponPushback;

        weaponAttach.forward = GameManager.CameraController.camera.transform.forward;
        // weaponAttach.position = transform.position + CameraController.Instance.camera.transform.forward + transform.TransformVector(weaponOffset) + pushback;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        
        SetSlot(0);

        // var spawned = GameManager.Spawn(GameManager.Instance.defaultGun);
        // spawned.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        // EquipGun_ClientRpc(spawned.GetComponent<Gun>());
    }

    private void LateUpdate()
    {
        if(weapon == null) return;
        
        UpdateWeaponPosition();
        // weapon.transform.position = weaponAttach.transform.position;
        weapon.transform.rotation = weaponAttach.transform.rotation;
    }

    [ClientRpc]
    public void EquipGun_ClientRpc(NetworkBehaviourReference weapon)
    {
        if (weapon.TryGet(out Gun gun))
        {
            this.weapon = gun;
            OnGunEquipped(null, gun);
        }
    }

    public void OnGunEquipped(Gun oldValue, Gun newValue)
    {
        if (oldValue)
        {
            foreach (Collider collider in oldValue.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
        }

        if (newValue == null)
        {
            return;
        }

        newValue.transform.parent = transform;
        newValue.transform.position = weaponAttach.transform.position;
        newValue.transform.rotation = weaponAttach.transform.rotation;
        foreach (Collider collider in newValue.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if(weapon == null) return;
        if(Application.isPlaying == false) return;
        weapon.GetComponent<NetworkObject>().Despawn();
    }

    public float maxPushback = .2f;

    public void AddWeaponPushback()
    {
        weaponPushback = maxPushback;
    }

    public bool HasWeapon()
    {
        return weapon;
    }

    public void AimAt(Vector3 lookAtPoint)
    {
        weapon.transform.LookAt(lookAtPoint);
        var gunRotation = weapon.transform.rotation.eulerAngles;
        if (gunRotation.x < 200f) gunRotation.x += 360f;
        // gunRotation.x = Mathf.Clamp(gunRotation.x, 300f, 370f);
        weapon.transform.rotation = Quaternion.Euler(gunRotation);
    }

    public void Aim(Vector3 aimPosition)
    {
        weapon.Aim(aimPosition);
    }

    public void StopAim()
    {
        weapon.StopAim();
    }

    public void SetSlot(int index)
    {
        if (weapon)
        {
            weapon.GetComponent<NetworkObject>().Despawn(); 
        }
        
        Gun spawned = GameManager.Spawner.Spawn(GameManager.Instance.guns[index]);
        spawned.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);
        EquipGun_ClientRpc(spawned.GetComponent<Gun>());
    }
}