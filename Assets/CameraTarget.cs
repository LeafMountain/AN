using UnityEngine;

public class CameraTarget : MonoBehaviour {
    public Transform target;

    private void Start() {
        target ??= transform;
        GameManager.CameraController.cinemachineVirtualCamera.Follow = target;
    }
}