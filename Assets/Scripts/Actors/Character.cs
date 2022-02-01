using Unity.Netcode;
using UnityEngine;

public class Character : Actor
{
    public DamageReciever damageReciever;
    public Gun weapon;
    public Transform weaponAttach;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            GameObject spawned = Instantiate(GameManager.Instance.defaultGun);
            spawned.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            EquipGun_ClientRpc(spawned.GetComponent<Gun>());
        }
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
        weapon.GetComponent<NetworkObject>().Despawn();
    }
}