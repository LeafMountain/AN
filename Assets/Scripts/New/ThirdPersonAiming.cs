using Unity.Cinemachine;
using UnityEngine;

public class ThirdPersonAiming : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineCamera mainCamera;
    public CinemachineCamera aimCamera;

    [Header("Settings")]
    public string aimInput = "Fire2"; // Right Mouse by default

    void Start()
    {
        SetAiming(false);
    }

    void Update()
    {
        bool isAiming = Input.GetButton(aimInput);
        SetAiming(isAiming);
    }

    void SetAiming(bool aiming)
    {
        mainCamera.Priority = aiming ? 5 : 10;
        aimCamera.Priority = aiming ? 15 : 5;
    }
}