using System;
using UnityEngine;

public class MultiMuzzleFire : MonoBehaviour, IFireMode
{
    public IMuzzleSelector muzzleSelector;
    public bool fireAllAtOnce = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;

    private void Awake() {
        muzzleSelector = GetComponent<IMuzzleSelector>();
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        if (fireAllAtOnce)
        {
            foreach (var muzzle in muzzleSelector.GetAllMuzzles())
                FireProjectile(muzzle.position, muzzle.forward);
        }
        else
        {
            var muzzle = muzzleSelector.GetNextMuzzle();
            FireProjectile(muzzle.position, muzzle.forward);
        }
    }

    private void FireProjectile(Vector3 position, Vector3 direction)
    {
        var proj = Instantiate(projectilePrefab, position, Quaternion.LookRotation(direction));
        proj.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;
    }
}