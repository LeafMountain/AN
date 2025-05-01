using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Player to follow
    [SerializeField] private Vector2 sensitivity = new Vector2(120f, 80f);
    [SerializeField] private Vector2 pitchLimits = new Vector2(-40f, 70f);
    [SerializeField] private float distance = 5f;

    private float yaw, pitch;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * sensitivity.x * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity.y * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f); // Look at head-height
    }
}
