using System;
using UnityEngine;

public class AttackAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 20f;

    private bool isAttacking = false;

    public bool IsActive => isAttacking;
    
    public static event Action OnAttack;

    public void Activate()
    {
        if (isAttacking) return;
        isAttacking = true;

        FaceCameraForward();
        PerformAttack();
        OnAttack?.Invoke(); 

        isAttacking = false;
    }

    public void Deactivate() { }
    public void Tick() { }

    private void FaceCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void PerformAttack()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, hitMask))
        {
            var targetHealth = hit.collider.GetComponent<IDamageable>();
            if (targetHealth != null)
                targetHealth.TakeDamage(damage);
        }

        Debug.DrawRay(origin, direction * attackRange, Color.red, 0.5f);
    }
}