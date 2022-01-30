using Mirror;
using UnityEngine;

public class Character : Actor
{
    public DamageReciever damageReciever;

    [SyncVar(hook = nameof(OnGunEquipped))]
    public Gun weapon;

    public Transform weaponAttach;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameObject spawned = Instantiate(GameManager.Instance.defaultGun);
        NetworkServer.Spawn(spawned, connectionToClient);
        weapon = spawned.GetComponent<Gun>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (weapon) OnGunEquipped(null, weapon);
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

        newValue.transform.parent = weaponAttach;
        newValue.transform.localPosition = Vector3.zero;
        newValue.transform.localRotation = Quaternion.identity;
        foreach (Collider collider in newValue.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
}