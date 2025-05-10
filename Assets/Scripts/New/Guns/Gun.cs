using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour, IOwnable {
    private IFireMode fireMode;
    private IMuzzle muzzle;
    private IAimProvider aimProvider;

    public float fireRate = 0.5f;
    private float lastFireTime;

    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    public bool IsReloading => isReloading;
    public GameObject Owner { get; set; }

    private void Awake() {
        fireMode = GetComponent<IFireMode>();
        muzzle = GetComponent<IMuzzle>();
        aimProvider = GetComponent<IAimProvider>();
        currentAmmo = maxAmmo; // Initialize with full ammo
    }

    public void Initialize(GameObject user) {
        Owner = user;
        fireMode.Initialize(this);
    }

    public void TryFire() {
        if (isReloading || currentAmmo <= 0 || Time.time - lastFireTime < fireRate)
            return;

        lastFireTime = Time.time;
        currentAmmo--;

        Vector3 direction = aimProvider.GetDirection();
        Vector3 origin = muzzle.GetMuzzlePosition();

        fireMode.Fire(origin, direction);
    }

    public void Reload() {
        if (isReloading || currentAmmo == maxAmmo)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        Debug.Log("Reloading");
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reloaded");
    }
}