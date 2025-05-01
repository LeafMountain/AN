using UnityEngine;

public class GunAimHandler : MonoBehaviour {
    [Header("References")] public Transform firePoint; // Where bullets spawn from
    public Camera aimCamera; // The camera to aim with
    public LayerMask aimLayerMask; // Optional: what to aim at (e.g., ground/enemies)

    [Header("Settings")] public float maxDistance = 1000f;

    public Vector3 AimDirection { get; private set; }
    public Vector3 AimPoint { get; private set; }

    private void Start() {
        aimCamera = CameraLocator.Instance.MainCamera;
    }

    public void UpdateAim() {
        Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimLayerMask)) {
            AimPoint = hit.point;
        }
        else {
            AimPoint = ray.origin + ray.direction * maxDistance;
        }

        AimDirection = (AimPoint - firePoint.position).normalized;
        firePoint.forward = AimDirection; // Rotate the fire point
    }
}