using System;
using Core;
using DefaultNamespace;
using EventManager;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    private static GameManager gameManager;
    public static GameManager Instance => gameManager ? gameManager : gameManager = FindFirstObjectByType<GameManager>();

    [Header("Managers")] [SerializeField] ItemManager itemManager;
    public static ItemManager ItemManager => Instance.itemManager;

    [SerializeField] Database database;
    public static Database Database => Instance.database;

    [SerializeField] CameraController cameraController;
    public static CameraController CameraController => Instance.cameraController;

    [SerializeField] Spawner spawner;
    public static Spawner Spawner => Instance.spawner;

    [SerializeField] AudioManager audioManager;
    public static AudioManager Audio => Instance.audioManager;

    [Header("Settings")] public Camera characterCameraPrefab;

    public Camera characterCamera;
    public GameObject defaultGun;
    public Player localPlayer;

    public Gun[] guns;

    public AudioSource audioInstancePrefab;
    public bool autoLockCursor;

    [SerializeField] private AudioManager.HitAudioProfile[] hitAudioProfiles;

    void Awake() {
        if (gameManager) return;

        gameManager = this;
        characterCamera ??= Spawner.Spawn(characterCameraPrefab);
    }

    void Start() {
        database.Init();

        Events.AddListener(Flag.DamageRecieved, OnDamageRecieved);
        LockCursor(autoLockCursor);
    }

    void OnDamageRecieved(object origin, EventArgs eventargs) {
        DamageRecievedArgs damageArgs = eventargs as DamageRecievedArgs;
        if (damageArgs.destroyed) return;
        if (damageArgs.instigator != localPlayer) return;
        CameraController.Shake(.2f, 4f);
    }

    public static void LockCursor(bool value) {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }
}