using System;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAbility : MonoBehaviour, IAbility
{
    [SerializeField] private PlayerEquipment playerEquipment;

    public bool IsActive { get; private set; }

    public void Activate()
    {
        Debug.Log("VAR");
        var currentGun = playerEquipment.CurrentGun;
        if (currentGun == null || currentGun.IsReloading) return;

        Debug.Log("VAR2");
        currentGun.Reload();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Tick()
    {
        var currentGun = playerEquipment.CurrentGun;
        if (currentGun.IsReloading) {
            return;
        }

        IsActive = false;
    }

    public IEnumerable<Type> GetConflictingAbilities()
    {
        // Reload conflicts with all other abilities
        return new List<Type> { typeof(RangedAttackAbility) };
    }
}