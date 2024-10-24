using System;
using Core;
using DefaultNamespace;
using EventManager;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour {
    static GameManager gameManager;
    public static GameManager Instance => gameManager ? gameManager : gameManager = FindFirstObjectByType<GameManager>();
    
    public static ItemManager ItemManager => Instance.itemManager;
    public static Database Database => Instance.database;
    public static CameraController CameraController => Instance.cameraController;
    public static Spawner Spawner => Instance.spawner;
    public static AudioManager Audio => Instance.audio;

    [Header("Managers")] 
    [SerializeField] ItemManager itemManager;
    [SerializeField] Database database;
    [SerializeField] CameraController cameraController;
    [SerializeField] Spawner spawner;
    [SerializeField] AudioManager audio;
    
    public Vector3 customSpawnLocation;
    public Quaternion customSpawnRotation;

    [Header("Settings")] public Camera characterCameraPrefab;

    public Camera characterCamera;
    public GameObject defaultGun;
    public Player localPlayer;

    public Gun[] guns;

    public AudioSource audioInstancePrefab;
    public bool autoLockCursor;


    void Awake() {
        if (gameManager) return;

        gameManager = this;
        characterCamera = spawner.Spawn(characterCameraPrefab, customSpawnLocation, customSpawnRotation);
    }

    void Start() {
        database.Init();

        Events.AddListener(Flag.DamageRecieved, OnDamageRecieved);
        LockCursor(autoLockCursor);
    }

    void OnDamageRecieved(object origin, EventArgs eventargs) {
        var damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed == false) {
            if (damageArgs.instigator == localPlayer) {
                CameraController.Shake(.2f, 4f);
            }
        }
        // if (damageArgs.destroyed)
        // {
        //     CameraController.Shake(.7f, 7f);
        // }
    }


    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null) {
        var spawned = Instantiate(original, position, rotation, parent);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false) {
            networkObject.Spawn();
        }

        return spawned;
    }

    public static T Spawn<T>(T original) where T : Component {
        var spawned = Instantiate(original);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false) {
            networkObject.Spawn();
        }

        return spawned;

        // return Instantiate(original);
    }

    public static T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null)
        where T : MonoBehaviour {
        return Instantiate(original, position, rotation, parent);
    }

    public static void LockCursor(bool value) {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    public static void Despawn(GameObject gameObject, float delay = 0) {
        if (delay == 0) {
            Destroy(gameObject);
        }
        else {
            Destroy(gameObject, delay);
        }
    }

    // Generic audio hit
    [Serializable]
    public struct HitAudioProfile {
        public PhysicsMaterial physicsMaterial;
        public AudioClip[] audioClips;
        public Vector2 pitchRange;
        public Vector2 volumeRange;
    }

    [SerializeField] private HitAudioProfile[] hitAudioProfiles;

    public static void PlayAudioByMaterial(PhysicsMaterial colliderMaterial, Vector3 position) {
        foreach (var instanceHitAudioProfile in Instance.hitAudioProfiles) {
            if (instanceHitAudioProfile.physicsMaterial != colliderMaterial) continue;
            var audioIndex = Random.Range(0, instanceHitAudioProfile.audioClips.Length - 1);
            PlayAudioInWorld(instanceHitAudioProfile.audioClips[audioIndex], position, instanceHitAudioProfile.pitchRange, instanceHitAudioProfile.volumeRange);
            break;
        }
    }

    public static void PlayAudioInWorld(AudioClip audioClip, Vector3 position, Vector2 pitchRange, Vector2 volumeRange) {
        var audioSource = GameManager.Spawn(GameManager.Instance.audioInstancePrefab);
        audioSource.transform.position = position;
        audioSource.PlayOneShot(audioClip);
        audioSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = UnityEngine.Random.Range(volumeRange.x, volumeRange.y);
        GameManager.Despawn(audioSource.gameObject, audioClip.length);
    }
}