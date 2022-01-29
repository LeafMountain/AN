using System;
using DG.Tweening;
using EventManager;
using Sirenix.OdinInspector;
using UnityEngine;

public class Gun : Actor 
{
    public Transform muzzlePoint;
    public Bullet bulletPrefab;
    public float bulletSpeed = 10f;

    [Button("Fire")]
    public void Fire()
    {
        var bullet = Instantiate(bulletPrefab, muzzlePoint.transform.position, muzzlePoint.transform.rotation);
        bullet.Init(this, bulletSpeed);
        Events.AddListener(Flag.BulletImpact, bullet, OnBulletHit);
        muzzlePoint.DOPunchScale(new Vector3(0f, 0f, -.1f), .1f);

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
                GameManager.Instance.characterVirtualCamera.Shake(.2f, 5f);
                // Debug.Log($"Bullet hit {impactArgs.hit.transform.name}");
                break;
            case Bullet.BulletImpactArgs.EventType.NoImpact:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}