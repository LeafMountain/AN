using System;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackAbility : MonoBehaviour, IAbility {
    // [SerializeField] private GameObject projectilePrefab;
    // [SerializeField] private Transform firePoint;
    // [SerializeField] private float projectileSpeed = 20f;

    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 20f;

    public bool IsActive { get; private set; }

    public void Activate() {
        // if (projectilePrefab == null || firePoint == null)
        // return;

        // var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // var rb = projectile.GetComponent<Rigidbody>();
        // if (rb != null)
        // rb.linearVelocity = firePoint.forward * projectileSpeed;

        IsActive = true;
    }

    public void Deactivate() {
        IsActive = false;
    }

    public void Tick() {
        if (IsActive == false) return;
        FaceCameraForward();
        playerEquipment.CurrentGun?.GetComponent<GunAimHandler>()?.UpdateAim();
        playerEquipment.CurrentGun?.TryFire();
    }

    public IEnumerable<Type> GetConflictingAbilities() {
        return new List<Type>();
    }

    private void FaceCameraForward() {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}