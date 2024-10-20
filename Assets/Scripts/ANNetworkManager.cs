using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ANNetworkManager : Unity.Netcode.NetworkManager
{
    public bool autoHost = true;

    private void Start()
    {
        if(autoHost)
            StartHost();
    }

    private void he()
    {
        var player = SpawnManager.GetLocalPlayerObject(); 
        GameManager.Instance.localPlayer = player.GetComponent<Player>();
        var controller = player.GetComponent<ThirdPersonController>(); 
        var input = player.GetComponent<PlayerInput>(); 
        var cameraFollower = FindObjectOfType<CinemachineVirtualCamera>();
        cameraFollower.Follow = controller.CinemachineCameraTarget.transform;
        
        controller.enabled = true;
        input.enabled = true;
    }

    [ContextMenu("Test shake camera")]
    public void TestShakeCamera()
    {
        GameManager.CameraController.Shake(1f, 5);
    }
}
