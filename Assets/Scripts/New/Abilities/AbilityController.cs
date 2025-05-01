using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> abilityBehaviours = new(); // Must implement IAbility
    private List<IAbility> abilities = new();

    private void Awake()
    {
        foreach (var mono in abilityBehaviours)
        {
            if (mono is IAbility ability)
                abilities.Add(ability);
        }
    }

    private void Update()
    {
        foreach (var ability in abilities)
            ability.Tick();
    }

    public void ActivateAbility<T>() where T : IAbility
    {
        foreach (var ability in abilities)
        {
            if (ability is T)
                ability.Activate();
        }
    }

    public void DeactivateAbility<T>() where T : IAbility
    {
        foreach (var ability in abilities)
        {
            if (ability is T)
                ability.Deactivate();
        }
    }
}