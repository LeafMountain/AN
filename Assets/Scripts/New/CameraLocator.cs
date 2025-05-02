using UnityEngine;

public class CameraLocator : MonoBehaviour
{
    public static CameraLocator Instance { get; private set; }

    public Camera MainCamera { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        MainCamera = GetComponentInChildren<Camera>();
        DontDestroyOnLoad(gameObject);
    }
}