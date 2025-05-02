using UnityEngine;

public class AbilityAnimationBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        // animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        AttackAbility.OnAttack += PlayAttack;
        JumpAbility.OnJump += PlayJump;
        // Add more as needed
    }

    private void OnDisable()
    {
        AttackAbility.OnAttack -= PlayAttack;
        JumpAbility.OnJump -= PlayJump;
    }

    private void PlayAttack() => animator.SetTrigger("Attack");
    private void PlayJump() => animator.SetTrigger("Jump");
}
