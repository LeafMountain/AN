public class Weapon_GrenadeLauncher_Projectile : Bullet 
{
    protected override void FixedUpdate()
    {
        // base.FixedUpdate();
        transform.Translate((targetPosition - transform.position).normalized * Time.fixedDeltaTime * speed);
    }

    protected override void OnCollision(Vector3 position, Vector3 normal, DamageReciever damageReciever)
    {
        base.OnCollision(position, normal, damageReciever);
        print("BOOM");
    }
}
