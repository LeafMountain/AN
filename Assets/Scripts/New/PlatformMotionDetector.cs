using UnityEngine;

public class PlatformMotionDetector : MonoBehaviour
{
    private PlatformMotionTracker motionTracker;

    void Awake()
    {
        motionTracker = GetComponentInParent<PlatformMotionTracker>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovingPlatform"))
            motionTracker.SetPlatform(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovingPlatform"))
            motionTracker.ClearPlatform();
    }
}