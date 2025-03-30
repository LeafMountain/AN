using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour {
    private const float _threshold = 0.01f;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public StarterAssetsInputs _input;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride;
    public bool LockCameraPosition;
    public PlayerInput _playerInput;
    public Transform lookAtTarget;
    private float _cinemachineTargetPitch;
    private float _cinemachineTargetYaw;
    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    public Camera camera => Camera.main;

    protected void LateUpdate() {
        CameraRotation();
    }

    private void OnValidate() {
        if (cinemachineVirtualCamera == null) cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFOV(float value) {
        DOTween.Kill("camera_fov");
        DOTween.To(() => cinemachineVirtualCamera.m_Lens.FieldOfView,
            x => cinemachineVirtualCamera.m_Lens.FieldOfView = x,
            value,
            .2f).SetId("camera_fov");
        // Instance.cinemachineVirtualCamera.m_Lens.FieldOfView = value;
    }

    public async void Shake(float time, float amplitude) {
        var basicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        basicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        var timer = time;

        while (timer > 0) {
            timer -= Time.deltaTime;
            var amp = Mathf.Lerp(0, amplitude, timer);
            basicMultiChannelPerlin.m_AmplitudeGain = amp;
            await Task.Yield();
        }
    }

    private void CameraRotation() {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
            //Don't multiply mouse input by Time.deltaTime;
            var deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        // cinemachineVirtualCamera.LookAt = lookAtTarget;
        cinemachineVirtualCamera.Follow.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        // cinemachineVirtualCamera.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }


    private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}