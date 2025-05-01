using UnityEngine;

public class VelocityAnimationBridge : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private float speed;

    // Optional: If using some custom movement system, you can pass this in
    public Vector3 CurrentVelocity => transform.position - lastPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Calculate speed based on position change (could be custom movement or input-based)
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        // Update the animator parameter to control locomotion animations
        animator.SetFloat("Speed", speed, .1f, Time.deltaTime);

        // Update last position for the next frame
        lastPosition = transform.position;

        // Optionally, you can track and apply rotation towards movement direction
        if (speed > 0.1f)
        {
            Vector3 direction = new Vector3(CurrentVelocity.x, 0, CurrentVelocity.z).normalized;
            if (direction != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime * 10f); // Rotate smoothly
            }
        }
    }
}