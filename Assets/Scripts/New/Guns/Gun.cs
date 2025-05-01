using UnityEngine;

public class Gun : MonoBehaviour {
    private IFireMode fireMode;
    private IMuzzle muzzle;
    private IAimProvider aimProvider;

    public float fireRate = 0.5f;
    private float lastFireTime;

    private void Awake() {
        fireMode = GetComponent<IFireMode>();
        muzzle = GetComponent<IMuzzle>();
        aimProvider = GetComponent<IAimProvider>();
    }

    public void TryFire() {
        if (Time.time - lastFireTime < fireRate)
            return;

        lastFireTime = Time.time;

        Vector3 direction = aimProvider.GetDirection();
        Vector3 origin = muzzle.GetMuzzlePosition();

        fireMode.Fire(origin, direction);
    }
}