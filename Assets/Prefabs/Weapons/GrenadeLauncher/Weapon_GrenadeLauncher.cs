using UnityEngine;

public class Weapon_GrenadeLauncher : Gun
{
    // Runtime
    public GameObject aimMarker;
    
    public override void Aim(Vector3 aimPosition)
    {
        base.Aim(aimPosition);
        this.aimPosition = aimPosition;
        aimMarker.gameObject.SetActive(true);
        aimMarker.transform.position = aimPosition;
    }

    public override void StopAim()
    {
        base.StopAim();
        aimMarker.gameObject.SetActive(false);
    }

    protected override void Fire_Internal()
    {
        if(aiming == false) return;

        Bullet grenade = SpawnBullet(); 
        // grenade.transform.DOMove(aimPosition, 1f);
    }
}
