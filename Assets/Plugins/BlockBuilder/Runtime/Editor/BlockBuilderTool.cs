using BlockBuilder;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[EditorTool("Block Builder Tool")]
public class BlockBuilderTool : EditorTool
{
    static Texture2D _toolIcon;

    readonly GUIContent _iconContent = new GUIContent
    {
        image = _toolIcon,
        text = "Block Builder",
        tooltip = "Block Builder Tool"
    };

    VisualElement toolRootElement;

    private BlockBuilderSettings settings;
    private BlockGridHolder gridHolder;
    private int currentBlockSetIndex = 0;
    private int blockSize => gridHolder.blockSize;

    bool _receivedClickDownEvent;
    bool _receivedClickUpEvent;
    private Vector3Int mouseDragPosition;
    private Vector3Int mouseGridPosition;

    bool HasPlaceableObject => settings != null;

    public override GUIContent toolbarIcon => _iconContent;

    public override void OnActivated()
    {
        settings = AssetDatabase.LoadAssetAtPath<BlockBuilderSettings>("Assets/Plugins/BlockBuilder/Resources/BlockBuilderSettings.asset");

        //Create the UI
        toolRootElement = new VisualElement();
        toolRootElement.style.width = 200;
        var backgroundColor = EditorGUIUtility.isProSkin
            ? new Color(0.21f, 0.21f, 0.21f, 0.8f)
            : new Color(0.8f, 0.8f, 0.8f, 0.8f);
        toolRootElement.style.backgroundColor = backgroundColor;
        toolRootElement.style.marginLeft = 10f;
        toolRootElement.style.marginBottom = 10f;
        toolRootElement.style.paddingTop = 5f;
        toolRootElement.style.paddingRight = 5f;
        toolRootElement.style.paddingLeft = 5f;
        toolRootElement.style.paddingBottom = 5f;
        var titleLabel = new Label("Block Builder Tool");
        titleLabel.style.unityTextAlign = TextAnchor.UpperCenter;

        // blockSetsSettings = new ObjectField {allowSceneObjects = false, objectType = typeof(BlockBuilderSettings)};
        // blockGridHolder = new ObjectField {allowSceneObjects = false, objectType = typeof(BlockGridHolder)};
        Selection.activeGameObject.TryGetComponent(out gridHolder);

        toolRootElement.Add(titleLabel);
        // toolRootElement.Add(blockGridHolder);

        string[] settingsAssets = AssetDatabase.FindAssets("t: BlockBuilderSettings");
        if (settingsAssets == null) return;
        // string assetPath = AssetDatabase.GUIDToAssetPath(settingsAssets[0]);
        // settings = AssetDatabase.LoadAssetAtPath<BlockBuilderSettings>(assetPath);

        UnityEditor.UIElements.Toolbar blockBar = new Toolbar();
        // blockBar.

        if (settings != null)
        {
            // var settings = (BlockBuilderSettings) blockSetsSettings.value;
            for (int i = 0; i < settings.blockSets.Length; i++)
            {
                var toggle = new ToolbarButton();
                toggle.name = settings.blockSets[i].name;
                toggle.style.width = 100f;

                blockBar.Add(toggle);
                // GUILayout.Button(settings.blockSets[i].name);
            }
        }

        toolRootElement.Add(blockBar);

        var sceneView = SceneView.lastActiveSceneView;
        sceneView.rootVisualElement.Add(toolRootElement);
        sceneView.rootVisualElement.style.flexDirection = FlexDirection.ColumnReverse;

        SceneView.beforeSceneGui += BeforeSceneGUI;
    }

    public override void OnWillBeDeactivated()
    {
        toolRootElement?.RemoveFromHierarchy();
        SceneView.beforeSceneGui -= BeforeSceneGUI;
    }

    void BeforeSceneGUI(SceneView sceneView)
    {
        if (!ToolManager.IsActiveTool(this)) return;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            _receivedClickDownEvent = true;
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            _receivedClickDownEvent = false;
            _receivedClickUpEvent = true;
            Event.current.Use();
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView)) return;
        if (!ToolManager.IsActiveTool(this)) return;
        if (!HasPlaceableObject) return;

        Event currentEvent = Event.current;
        Vector3 mousePosition = currentEvent.mousePosition;
        int blocksPerTile = settings.blockSets[currentBlockSetIndex].blocksPerTile;

        //Draw a positional Handle.
        Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.up, 0.5f);
        if (gridHolder != null)
        {
            Vector3 worldPosition = gridHolder.transform.position;

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
            // if (Physics.Raycast(mouseRay, out var hit))
            // {
            //     float normalMultiplier = blockSize * .5f;
            //     mousePosition = hit.point + (currentEvent.control ? -hit.normal : hit.normal) * normalMultiplier;
            //     // mousePosition = hit.point + hit.normal * normalMultiplier;
            //     // Debug.DrawLine(hit.point, mousePosition, Color.red);
            // }
            // else
            {
                mousePosition = GetPointOnPlane(mouseRay.origin, mouseRay.direction, worldPosition);
            }

            mousePosition += Vector3.one * .01f;
            int markerSize = blocksPerTile * blockSize;

            mouseGridPosition = BlockGridHolder.WorldToGridPosition(gridHolder, mousePosition, -1, blocksPerTile);
            mousePosition = BlockGridHolder.GridToWorldPosition(gridHolder, mouseGridPosition, -1, blocksPerTile);

            Handles.color = currentEvent.control ? Color.red : Color.cyan;
            Handles.DrawWireCube(mousePosition, Vector3.one * markerSize);
        }

        Vector3Int fromPoint = new Vector3Int(Mathf.Min(mouseGridPosition.x, mouseDragPosition.x), Mathf.Min(mouseGridPosition.y, mouseDragPosition.y), Mathf.Min(mouseGridPosition.z, mouseDragPosition.z));
        Vector3 fromPointWorld = BlockGridHolder.GridToWorldPosition(gridHolder, fromPoint);
        Vector3Int toPoint = new Vector3Int(Mathf.Max(mouseGridPosition.x, mouseDragPosition.x), Mathf.Max(mouseGridPosition.y, mouseDragPosition.y), Mathf.Max(mouseGridPosition.z, mouseDragPosition.z));
        Vector3 toPointWorld = BlockGridHolder.GridToWorldPosition(gridHolder, toPoint);
        // Handles.color = Color.yellow;

        fromPointWorld -= Vector3.one * blockSize * blocksPerTile * .5f;
        toPointWorld += Vector3.one * blockSize * blocksPerTile * .5f;
        var areaMarkerPosition = new Vector3(Mathf.Abs(toPointWorld.x - fromPointWorld.x), Mathf.Abs(toPointWorld.y - fromPointWorld.y),Mathf.Abs(toPointWorld.z - fromPointWorld.z));
        Handles.DrawWireCube(fromPointWorld + (toPointWorld - fromPointWorld) * .5f,areaMarkerPosition);

        if (_receivedClickDownEvent == false)
        {
            mouseDragPosition = mouseGridPosition;
        }

        if (_receivedClickUpEvent)
        {
            if (Event.current.control)
            {
                Undo.RecordObject(gridHolder, "Removed Blocks");
                
                for (int y = fromPoint.y; y <= toPoint.y; y++)
                for (int z = fromPoint.z; z <= toPoint.z; z++)
                for (int x = fromPoint.x; x <= toPoint.x; x++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);
                    RemoveBlock(gridPosition);
                }
            }
            else
            {
                Undo.RecordObject(gridHolder, "Placed Blocks");

                for (int y = fromPoint.y; y <= toPoint.y; y++)
                for (int z = fromPoint.z; z <= toPoint.z; z++)
                for (int x = fromPoint.x; x <= toPoint.x; x++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);
                    PlaceBlock(gridPosition);
                }
            }

            _receivedClickUpEvent = false;

            gridHolder.Run();
        }

        //Force the window to repaint.
        window.Repaint();
    }

    private void RemoveBlock(Vector3Int mouseGridPosition, int selectedSet = 0)
    {
        gridHolder.RemoveBlock(mouseGridPosition, settings.blockSets[selectedSet].blocksPerTile);
    }

    private void PlaceBlock(Vector3Int gridPosition, int selectedSet = 0)
    {
        gridHolder.PlaceBlock(gridPosition, selectedSet, settings.blockSets[selectedSet].blocksPerTile);
    }

    private static Vector3 GetPointOnPlane(Vector3 origin, Vector3 forward, Vector3 planeOrigin)
    {
        Vector3 normal = Vector3.up * -1f;
        float denominator = Vector3.Dot(forward, normal);
        if (denominator > 0.00001f)
        {
            float distanceToPlane = Vector3.Dot(planeOrigin - origin, normal) / denominator;
            Vector3 intersectionPoint = origin + forward * distanceToPlane;
            return intersectionPoint;
        }

        return Vector3.zero;
    }

    Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        var placeObject = HandleUtility.PlaceObject(mousePosition, out var newPosition, out var normal);
        return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }

    // void ShowMenu()
    // {
    //     var picked = HandleUtility.PickGameObject(Event.current.mousePosition, true);
    //     if (!picked) return;
    //
    //     var menu = new GenericMenu();
    //     // menu.AddItem(new GUIContent($"Pick {picked.name}"), false, () => { _prefabObjectField.value = picked; });
    //     // menu.AddItem(new GUIContent($"Pick {picked.name}"), false, () => { settings.value = picked; });
    //     menu.ShowAsContext();
    // }
}