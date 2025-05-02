using UnityEngine;

public class HitscanFire : MonoBehaviour, IFireMode
{
    public int damage = 10;
    public float range = 100f;

    private Gun gun;

    public void Initialize(Gun gun) {
        this.gun = gun;
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            hit.collider.GetComponent<IDamageable>()?.TakeDamage(damage, gun.Owner);
        }

        Debug.DrawRay(origin, direction * range, Color.red, 1f);
    }
}