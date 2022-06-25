using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraController : MonoBehaviour 
{
    public static CameraController Instance { get; private set; }

    private void Awake() { Instance = this; }

    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Camera camera => Camera.main;

    private void OnValidate()
    {
        if (cinemachineVirtualCamera == null) cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }


    public static void SetFOV(float value)
    {
        Instance.cinemachineVirtualCamera.m_Lens.FieldOfView = value;
    }

    public static async void Shake(float time, float amplitude)
    {
        var basicMultiChannelPerlin = Instance.cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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