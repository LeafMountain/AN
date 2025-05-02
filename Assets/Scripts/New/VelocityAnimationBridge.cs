using UnityEngine;

[RequireComponent(typeof(Animator))]
public class VelocityAnimationBridge : MonoBehaviour
{
    [SerializeField] private float dampTime = 0.1f; // Damping time for animation transitions
    private Animator animator;
    private Vector3 lastPosition;
    private Vector3 velocityWorld;
    private Vector3 velocityLocal;

    public Vector3 WorldVelocity => velocityWorld;
    public Vector3 LocalVelocity => velocityLocal;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        velocityWorld = (transform.position - lastPosition) / Time.deltaTime;
        velocityLocal = transform.InverseTransformDirection(velocityWorld);

        animator.SetFloat("VelocityX", velocityLocal.x, dampTime, Time.deltaTime); // Strafe
        animator.SetFloat("VelocityZ", velocityLocal.z, dampTime, Time.deltaTime); // Forward/Backward
        animator.SetFloat("VerticalSpeed", velocityWorld.y, dampTime, Time.deltaTime); // Jump/Fall

        lastPosition = transform.position;
    }
}