using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController controller;
    public bool IsActive { get; private set; }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void Tick()
    {
        if (!IsActive || cameraTransform == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.sqrMagnitude > 0.01f)
        {
            // Camera-relative direction
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * v + camRight * h).normalized;

            // Apply movement
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Rotate toward movement direction
            // Quaternion targetRotation = Quaternion.LookRotation((camForward + camRight));
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            FaceCameraForward();
        }
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