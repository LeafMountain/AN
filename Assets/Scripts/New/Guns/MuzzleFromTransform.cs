using UnityEngine;

public class MuzzleFromTransform : MonoBehaviour, IMuzzle
{
    public Transform muzzleTransform;
    public Vector3 GetMuzzlePosition() => muzzleTransform.position;
}