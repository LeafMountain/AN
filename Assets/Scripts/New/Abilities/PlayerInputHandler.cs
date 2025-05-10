using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private AbilityController abilityController;

    private void Awake()
    {
        abilityController = GetComponent<AbilityController>();
    }

    private void Update()
    {
        // Movement is continuous; ensure it's always active
        abilityController.ActivateAbility<MoveAbility>();
        abilityController.ActivateAbility<LootMagnetizerAbility>();
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
            abilityController.ActivateAbility<SprintAbility>();
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            abilityController.DeactivateAbility<SprintAbility>();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
            abilityController.ActivateAbility<JumpAbility>();

        // Melee attack
        if (Input.GetMouseButtonDown(1))
            abilityController.ActivateAbility<AttackAbility>();
        
        // Reload
        if (Input.GetKeyDown(KeyCode.R))
            abilityController.ActivateAbility<ReloadAbility>();

        // Ranged attack
        if (Input.GetMouseButtonDown(0))
            abilityController.ActivateAbility<RangedAttackAbility>();
        else if (Input.GetMouseButtonUp(0))
            abilityController.DeactivateAbility<RangedAttackAbility>();

        // Pickup
        if (Input.GetKeyDown(KeyCode.E))
            abilityController.ActivateAbility<PickupAbility>();

        if (Input.GetKeyDown(KeyCode.LeftControl)) 
            abilityController.ActivateAbility<DashAbility>();
    }
}