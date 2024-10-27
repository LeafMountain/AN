using System;
using Core;
using DefaultNamespace;
using EventManager;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    static GameManager gameManager;
    public static GameManager Instance => gameManager ? gameManager : gameManager = FindFirstObjectByType<GameManager>();
    
    public static ItemManager ItemManager => Instance.itemManager;
    public static Database Database => Instance.database;
    public static CameraController CameraController => Instance.cameraController;
    public static Spawner Spawner => Instance.spawner;
    public static AudioManager Audio => Instance.audio;
    public static UI.UI UI => Instance.ui;
    public static Players Players => Instance.players;

    [Header("Managers")] 
    [SerializeField] ItemManager itemManager;
    [SerializeField] Database database;
    [SerializeField] CameraController cameraController;
    [SerializeField] Spawner spawner;
    [SerializeField] AudioManager audio;
    [SerializeField] UI.UI ui;
    [SerializeField] Players players;
    
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
        characterCamera ??= spawner.Spawn(characterCameraPrefab, customSpawnLocation, customSpawnRotation);
    }

    void Start() {
        database.Init();
        ui.Init();

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

    public static void LockCursor(bool value) {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }
}