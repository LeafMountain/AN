using Unity.Netcode;
using UnityEngine;

public class Character : Actor
{
    public DamageReciever damageReceiver;
    public Gun weapon;
    public Transform weaponAttach;
    public Animator animator;

    private Vector3 positionLastFrame;
    public Vector3 Velocity { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            var spawned = GameManager.Spawn(GameManager.Instance.defaultGun);
            spawned.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            EquipGun_ClientRpc(spawned.GetComponent<Gun>());
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected virtual void LateUpdate()
    {
        UpdateVelocity();
        UpdateAnimatorValues();
    }

    protected virtual void FixedUpdate()
    {
    }

    private void UpdateVelocity()
    {
        Vector3 position = transform.position;
        Velocity = position - positionLastFrame;
        positionLastFrame = position;
    }

    private void UpdateAnimatorValues()
    {
        if(animator == null) return;

        Vector3 localVelocity = transform.InverseTransformDirection(Velocity) / Time.deltaTime;
        animator.SetFloat("HorizontalSpeed", localVelocity.x);
        animator.SetFloat("Speed", localVelocity.z);
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