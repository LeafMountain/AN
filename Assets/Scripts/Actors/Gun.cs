using System;
using System.Threading.Tasks;
using DG.Tweening;
using EventManager;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Gun : Actor 
{
    public Transform muzzlePoint;
    public Bullet bulletPrefab;
    public float bulletSpeed = 10f;
    public float bulletsPerSecond = 10f;

    private bool onCooldown;

    [Button("Fire")]
    public void Fire()
    {
        if(onCooldown) return;
        
        var bullet = Instantiate(bulletPrefab, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
        bullet.Init(this, bulletSpeed);
        Events.AddListener(Flag.BulletImpact, bullet, OnBulletHit);
        muzzlePoint.DOPunchScale(new Vector3(0f, 0f, -.1f), .1f);
        StartFireRateCooldown(1f / bulletsPerSecond);
        bullet.GetComponent<NetworkObject>().Spawn(); 
        // NetworkServer.Spawn(bullet.gameObject);
    }

    private async void StartFireRateCooldown(float cooldown)
    {
        onCooldown = true;
        await Task.Delay((int)(cooldown * 1000));
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
}