using UnityEngine;

public class Weapon_GrenadeLauncher_Projectile : Bullet 
{
    protected override void FixedUpdate()
    {
        // base.FixedUpdate();
        transform.Translate((targetPosition - transform.position).normalized * Time.fixedDeltaTime * speed);
    }

    protected override void OnCollision(Vector3 position, Vector3 normal, DamageReceiver damageReceiver)
    {
        base.OnCollision(position, normal, damageReceiver);
        print("BOOM");
    }
}
