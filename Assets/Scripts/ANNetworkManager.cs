using Mirror;
using UnityEngine;

public class ANNetworkManager : NetworkManager {
    public bool autoHost = true;

    void Start() {
        StartHost();
    }

    // private void he() {
    //     var player = SpawnManager.GetLocalPlayerObject();
    //     GameManager.Instance.localPlayer = player.GetComponent<Player>();
    //     var controller = player.GetComponent<ThirdPersonController>();
    //     var input = player.GetComponent<PlayerInput>();
    //     var cameraFollower = FindObjectOfType<CinemachineVirtualCamera>();
    //     cameraFollower.Follow = controller.CinemachineCameraTarget.transform;
    //
    //     controller.enabled = true;
    //     input.enabled = true;
    // }

    [ContextMenu("Test shake camera")]
    public void TestShakeCamera() {
        GameManager.CameraController.Shake(1f, 5);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F7)) {
            StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.F8)) {
            StartClient();
        }
    }
}