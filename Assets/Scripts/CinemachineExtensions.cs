// using System.Collections;
// using System.Collections.Generic;
// using System.Security.Cryptography.X509Certificates;
// using System.Threading.Tasks;
// using Cinemachine;
// using UnityEngine;
// using UnityEngine.UI;
//
// public static class CinemachineExtensions
// {
//     public static async void Shake(this CinemachineVirtualCamera camera, float time, float amplitude)
//     {
//         var basicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
//         basicMultiChannelPerlin.m_AmplitudeGain = amplitude;
//         float timer = time;
//         
//         while (timer > 0)
//         {
//             timer -= Time.deltaTime;
//             float amp = Mathf.Lerp(0, amplitude, timer);
//             basicMultiChannelPerlin.m_AmplitudeGain = amp;
//             await Task.Yield();
//         }
//     }
// }