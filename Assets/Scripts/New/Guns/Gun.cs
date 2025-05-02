using UnityEngine;

public class Gun : MonoBehaviour, IOwnable {
    private IFireMode fireMode;
    private IMuzzle muzzle;
    private IAimProvider aimProvider;

    public float fireRate = 0.5f;
    private float lastFireTime;
    
    public GameObject Owner { get; set; }

    private void Awake() {
        fireMode = GetComponent<IFireMode>();
        muzzle = GetComponent<IMuzzle>();
        aimProvider = GetComponent<IAimProvider>();
    }

    public void Initialize(GameObject user) {
        Owner = user;
        fireMode.Initialize(this);
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