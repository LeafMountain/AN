using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour 
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Camera camera => Camera.main;

    private void OnValidate()
    {
        if (cinemachineVirtualCamera == null) cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFOV(float value)
    {
        DOTween.Kill("camera_fov");
        DOTween.To(() => cinemachineVirtualCamera.m_Lens.FieldOfView, 
            x => cinemachineVirtualCamera.m_Lens.FieldOfView = x, 
            value, 
            .2f).SetId("camera_fov");
        // Instance.cinemachineVirtualCamera.m_Lens.FieldOfView = value;
    }

    public async void Shake(float time, float amplitude)
    {
        var basicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        basicMultiChannelPerlin.m_AmplitudeGain = amplitude;
        float timer = time;
        
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float amp = Mathf.Lerp(0, amplitude, timer);
            basicMultiChannelPerlin.m_AmplitudeGain = amp;
            await Task.Yield();
        }
    }
}