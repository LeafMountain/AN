using UnityEngine;

public class TransformForwardAim : MonoBehaviour, IAimProvider
{
    public Transform aimTransform;
    public Vector3 GetDirection() => aimTransform.forward;
}