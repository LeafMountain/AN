using System;
using System.Threading.Tasks;
using Cinemachine;
using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkManager : Mirror.NetworkManager 
{
    public override async void OnStartClient()
    {
        base.OnStartClient();

        await Task.Delay(1000);
        
        var player = NetworkClient.localPlayer.gameObject;
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
        var cameraFollower = FindObjectOfType<CinemachineVirtualCamera>();
        cameraFollower.Shake(1f, 5);
    }
}
