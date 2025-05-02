using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraControllerCinemachine : MonoBehaviour {
    public List<CinemachineCamera> virtualCameras;
    public Transform playerTransform;

    public float rotationSpeed = 3f;
    public float pitchMin = -30f;
    public float pitchMax = 60f;

    private float yaw = 0f;
    private float pitch = 20f;

    private Transform cameraFollowTarget;

    void Start() {
        cameraFollowTarget = new GameObject("CameraFollowTarget").transform;
        cameraFollowTarget.position = playerTransform.position;
        foreach (var virtualCamera in virtualCameras) {
            virtualCamera.Follow = cameraFollowTarget;
            virtualCamera.LookAt = playerTransform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        float mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Apply rotation
        cameraFollowTarget.position = playerTransform.position;
        cameraFollowTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}