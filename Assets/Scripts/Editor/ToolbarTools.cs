
    [EditorToolbarElement(id, typeof(SceneView))]
    class PlayFromHere : EditorToolbarToggle
    {
        public const string prefs = "editorPlayFromSceneCamera";
        public const string id = "AlexToolBar/PlayFromHere";
        // public static bool enabled => PlayerPrefs.HasKey(prefs);

        public PlayFromHere()
        {
            text = "Play From Here";
            icon = Resources.Load<Texture2D>("AlexToolCamera");
            SceneView.duringSceneGui += OnSceneGui;
            value = PlayerPrefs.HasKey(prefs);
        }

        private void OnSceneGui(SceneView view)
        {
            if (value)
            {
                PlayerPrefs.SetInt(prefs, 0);
            }
            else
            {
                PlayerPrefs.DeleteKey(prefs);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void TeleportPlayer()
        {
            if (PlayerPrefs.HasKey(prefs))
            {
                GameManager.Instance.customSpawnLocation = SceneView.lastActiveSceneView.camera.transform.position;
            }
        }
    }

    [Overlay(typeof(SceneView), "AlexToolBar")]
    public class AlexToolBar : ToolbarOverlay
    {
        // ToolbarOverlay implements a parameterless constructor, passing the EditorToolbarElementAttribute ID.
        // This is the only code required to implement a toolbar Overlay. Unlike panel Overlays, the contents are defined
        // as standalone pieces that will be collected to form a strip of elements.

        AlexToolBar() : base(PlayFromHere.id)
                  {}
                  }
    
