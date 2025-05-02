using UnityEngine;

public class PlatformMotionTracker : MonoBehaviour
{
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    public void SetPlatform(Transform platform)
    {
        currentPlatform = platform;
        lastPlatformPosition = platform ? platform.position : Vector3.zero;
    }

    void LateUpdate()
    {
        if (currentPlatform == null) return;

        Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
        transform.position += platformDelta;

        lastPlatformPosition = currentPlatform.position;
    }

    public void ClearPlatform()
    {
        currentPlatform = null;
    }
}