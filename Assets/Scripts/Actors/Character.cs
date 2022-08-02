using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Character : Actor
{
    public DamageReceiver damageReceiver;
    public Gun weapon;
    public Transform weaponAttach;
    public Animator animator;

    public SkinnedMeshRenderer leg;

    private Vector3 positionLastFrame;
    public Vector3 Velocity { get; private set; }

    public SkinnedMeshRenderer testPart;

    protected override void Start()
    {
        base.Start(); 
        
        animator.Rebind();
    }
    
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
        UpdateWeaponPosition();

        weapon.transform.position = weaponAttach.transform.position;
        weapon.transform.rotation = weaponAttach.transform.rotation;
    }

    protected virtual void FixedUpdate()
    {
    }

    public Vector3 weaponOffset = Vector3.forward;

    private void UpdateWeaponPosition()
    {
        weaponPushback -= Time.deltaTime * 2f;
        weaponPushback = math.clamp(weaponPushback, 0, float.MaxValue);

        var pushback = weaponAttach.TransformVector(Vector3.back) * weaponPushback;
        
        weaponAttach.forward = CameraController.Instance.camera.transform.forward;
        weaponAttach.position = transform.position + CameraController.Instance.camera.transform.forward + transform.TransformVector(weaponOffset) + pushback;
    }

    private float weaponPushback = 0f;
    public float maxPushback = .2f;
    
    public void AddWeaponPushback()
    {
        weaponPushback = maxPushback;
    }

    private void UpdateVelocity()
    {
        Vector3 position = transform.position;
        Velocity = position - positionLastFrame;
        positionLastFrame = position;
    }

    private void UpdateAnimatorValues()
    {
        if (animator == null) return;

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