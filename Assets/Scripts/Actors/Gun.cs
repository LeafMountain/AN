using System;
using System.Threading.Tasks;
using DG.Tweening;
using EventManager;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Gun : Actor
{
    private int currentMuzzleIndex = 0;
    public Transform[] muzzlePoints;
    public Transform muzzlePoint => muzzlePoints[currentMuzzleIndex = (currentMuzzleIndex + 1) % muzzlePoints.Length];
        
    [FormerlySerializedAs("bulletPrefab")] public Bullet projectilePrefab;
    public float bulletSpeed = 10f;
    public float bulletsPerSecond = 10f;

    // Runtime
    [Header("Runtime")] public bool aiming;
    private bool onCooldown;

    protected Vector3 aimPosition;

    public virtual void Aim(Vector3 aimPosition)
    {
        aiming = true;
        this.aimPosition = aimPosition;
        CameraController.SetFOV(25);
    }

    [Button("Fire")]
    public void Fire()
    {
        if (onCooldown) return;
        StartFireRateCooldown(1f / bulletsPerSecond);
        
        Fire_Internal();
    }

    protected virtual void Fire_Internal()
    {
        SpawnBullet();
        muzzlePoint.DOPunchScale(new Vector3(0f, 0f, -.1f), .1f);
    }

    protected Bullet SpawnBullet()
    {
        Bullet bullet = Instantiate(projectilePrefab, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
        bullet.Init(this, bulletSpeed, aimPosition);
        Events.AddListener(Flag.BulletImpact, bullet, OnBulletHit);
        bullet.GetComponent<NetworkObject>().Spawn();
        return bullet;
    }

    private async void StartFireRateCooldown(float cooldown)
    {
        onCooldown = true;
        await Task.Delay((int) (cooldown * 1000));
        onCooldown = false;
    }

    private void OnBulletHit(object origin, EventArgs eventargs)
    {
        Events.RemoveListener(Flag.BulletImpact, origin, OnBulletHit);
        var impactArgs = eventargs as Bullet.BulletImpactArgs;
        switch (impactArgs.eventType)
        {
            case Bullet.BulletImpactArgs.EventType.None:
                break;
            case Bullet.BulletImpactArgs.EventType.Impact:
                // Do damage
                // GameManager.Instance.characterVirtualCamera.Shake(.2f, 5f);
                // Debug.Log($"Bullet hit {impactArgs.hit.transform.name}");
                break;
            case Bullet.BulletImpactArgs.EventType.NoImpact:
                break;
        }
    }

    public virtual void StopAim()
    {
        aiming = false;
        CameraController.SetFOV(40);
    }
}