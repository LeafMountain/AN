using UnityEngine;

public class DashAbility : MonoBehaviour, IAbility
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private Transform cameraTransform;

    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    public bool IsActive => isDashing;

    public void Activate()
    {
        if (Time.time < lastDashTime + dashCooldown || isDashing) return;

        isDashing = true;
        Vector3 dashDirection = GetDashDirection();
        PerformDash(dashDirection);
        lastDashTime = Time.time;
        isDashing = false;
    }

    public void Deactivate() { }

    public void Tick() { }

    private Vector3 GetDashDirection()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private void PerformDash(Vector3 direction)
    {
        Vector3 dashPosition = transform.position + direction * dashDistance;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, dashDistance))
        {
            dashPosition = hit.point; // Stop at the obstacle
        }
        // transform.position = dashPosition;
        characterController.Move(dashPosition - transform.position);
        Debug.Log($"Dashing to {dashPosition}");
    }
}