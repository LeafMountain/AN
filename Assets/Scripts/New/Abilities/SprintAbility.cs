using UnityEngine;

public class SprintAbility : MonoBehaviour, IAbility
{
    [SerializeField] private float sprintMultiplier = 1.5f;

    private MoveAbility moveAbility;
    private float originalSpeed;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        moveAbility = GetComponent<MoveAbility>();
        if (moveAbility != null)
            originalSpeed = moveAbility.MoveSpeed;
    }

    public void Activate()
    {
        if (moveAbility == null || IsActive) return;

        moveAbility.MoveSpeed = originalSpeed * sprintMultiplier;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (moveAbility == null || !IsActive) return;

        moveAbility.MoveSpeed = originalSpeed;
        IsActive = false;
    }

    public void Tick() { }
}