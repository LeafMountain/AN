using UnityEngine;

public static class CombatEvents
{
    public static event System.Action<Vector3> OnHitConfirmed;
    public static event System.Action<Vector3> OnTargetKilled;

    public static void Hit(Vector3 screenPosition) => OnHitConfirmed?.Invoke(screenPosition);
    public static void Kill(Vector3 screenPosition) => OnTargetKilled?.Invoke(screenPosition);
}