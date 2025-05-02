using UnityEngine;

public class GroundedAnimationMonitor : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    private Animator animator;

    // Optional: If using some custom movement system, you can pass this in
    public bool IsGrounded => characterController.isGrounded;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool("Grounded", IsGrounded);
    }
}