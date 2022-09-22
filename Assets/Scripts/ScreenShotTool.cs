using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ScreenShotTool : EditorWindow
{
    private Camera camera;
    private Vector2Int resolution = new Vector2Int(3840, 2160);

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Screenshot Tool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ScreenShotTool window = (ScreenShotTool)EditorWindow.GetWindow(typeof(ScreenShotTool));
        window.Show();
    }

    void OnGUI()
    {
        string folderPath = $"{Application.dataPath}/Screenshots/";

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        camera = (Camera)EditorGUILayout.ObjectField("Camera", camera, typeof(Camera));
        resolution = EditorGUILayout.Vector2IntField("Resolution", resolution);

        if (GUILayout.Button("Take screeshot"))
        {
            // ScreenCapture.CaptureScreenshot(filename, scaleFactor);
            RenderTexture renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
            camera.targetTexture = renderTexture;
            Texture2D screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(renderTexture);
            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.Directory.CreateDirectory(folderPath);
            string filename = $"{folderPath}screen_{resolution.x}x{resolution.y}_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log($"Screenshot placed in: {filename}");
        }

        if (GUILayout.Button("Open Folder"))
        {
            Process.Start($@"{folderPath}");
        }
    }
}
