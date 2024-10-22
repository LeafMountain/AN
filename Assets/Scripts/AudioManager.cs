using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] HitAudioProfile[] hitAudioProfiles;
        [SerializeField] AudioSource audioInstancePrefab;
        
        // Generic audio hit
        [Serializable]
        public struct HitAudioProfile
        {
            public PhysicsMaterial physicsMaterial;
            public AudioClip[] audioClips;
            public Vector2 pitchRange;
            public Vector2 volumeRange;
        }

        public void PlayAudioByMaterial(PhysicsMaterial colliderMaterial, Vector3 position)
        {
            foreach (var instanceHitAudioProfile in hitAudioProfiles)
            {
                if (instanceHitAudioProfile.physicsMaterial != colliderMaterial) continue;
                var audioIndex = Random.Range(0, instanceHitAudioProfile.audioClips.Length - 1);
                PlayAudioInWorld(instanceHitAudioProfile.audioClips[audioIndex], position, instanceHitAudioProfile.pitchRange, instanceHitAudioProfile.volumeRange);
                break;
            }
        }

        public void PlayAudioInWorld(AudioClip audioClip, Vector3 position, Vector2 pitchRange, Vector2 volumeRange)
        {
            AudioSource audioSource = GameManager.Spawner.Spawn(audioInstancePrefab);
            audioSource.transform.position = position;
            audioSource.PlayOneShot(audioClip);
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
            GameManager.Spawner.Despawn(audioSource.gameObject, audioClip.length);
        }
    }
}