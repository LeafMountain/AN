namespace BlockBuilder
{
    // [CustomEditor(typeof(BlockGridExampleCreator))]
    // public class BlockGridExampleCreatorEditor : Editor
    // {
    //     private BlockBuilderSettings settings;
    //     // private SerializedObject serializedDatabase;
    //     // private SerializedProperty databaseProperty;
    //     private int selectedSet;
    //     private int selectedBlock;
    //     private BlockGridExampleCreator targetGridCreator;
    //     private GameObject previewBlock;
    //     private int previousSelectedBlock;
    //     private int rotation = 0;
    //
    //     private void SetupReferences()
    //     {
    //         string[] settingsAssets = AssetDatabase.FindAssets("t: BlockBuilderSettings");
    //         if (settingsAssets == null) return;
    //         string assetPath = AssetDatabase.GUIDToAssetPath(settingsAssets[0]);
    //         settings = AssetDatabase.LoadAssetAtPath<BlockBuilderSettings>(assetPath);
    //         if (settings == null) return;
    //         // serializedDatabase = new SerializedObject(settings);
    //         // databaseProperty = serializedDatabase.FindProperty("blockSets");
    //     }
    //
    //     private void OnSceneGUI()
    //     {
    //         if(settings == null) SetupReferences();
    //         targetGridCreator = (BlockGridExampleCreator) base.target;
    //         // Handles.matrix = target.transform.localToWorldMatrix;
    //         Handles.color = new Color(1f, .4f, .5f, .1f);
    //
    //         // Draw marker
    //         Event currentEvent = Event.current;
    //         Vector3 mousePosition = currentEvent.mousePosition;
    //         Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
    //         RaycastHit hit = default;
    //         if (Physics.Raycast(mouseRay, out hit))
    //         {
    //             float normalMultiplier = targetGridCreator.blockSize;
    //             mousePosition = hit.point + ((currentEvent.control ? -hit.normal : hit.normal));
    //             Debug.DrawLine(hit.point, mousePosition, Color.red);
    //         }
    //         else
    //         {
    //             mousePosition = GetPointOnPlane(mouseRay.origin, mouseRay.direction, targetGridCreator.transform.position);
    //         }
    //
    //         int blocksPerTile = settings.blockSets[selectedSet].blocksPerTile;
    //         int markerSize = blocksPerTile * (int)targetGridCreator.blockSize;
    //
    //         Vector3Int mouseGridPosition = WorldToGridPosition(mousePosition, blocksPerTile);
    //         mousePosition = GridToWorldPosition(mouseGridPosition, blocksPerTile);
    //         
    //         Handles.color = currentEvent.control ? Color.red : Color.cyan;
    //         Handles.DrawWireCube(mousePosition, Vector3.one * targetGridCreator.blockSize);
    //
    //         if (previewBlock)
    //         {
    //             previewBlock.transform.position = mousePosition;
    //             previewBlock.transform.rotation = Quaternion.Euler(0f, rotation * 90f, 0f);
    //         }
    //
    //         // Place
    //         switch (currentEvent.type)
    //         {
    //             case EventType.MouseDrag:
    //             case EventType.MouseDown:
    //                 if (currentEvent.button == 0)
    //                 {
    //                     if (currentEvent.control)
    //                     {
    //                         if(hit.transform) RemoveBlock(hit.transform.gameObject);
    //                     }
    //                     else
    //                     {
    //                         PlaceBlock(mouseGridPosition, Quaternion.Euler(0f, rotation * 90f, 0f), targetGridCreator.transform);
    //                     }
    //                     
    //                     currentEvent.Use();
    //                 }
    //
    //                 break;
    //             case EventType.Layout:
    //                 HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
    //                 break;
    //             case EventType.KeyDown:
    //                 if (currentEvent.isKey)
    //                 {
    //                     if(currentEvent.keyCode == KeyCode.Q)
    //                     {
    //                         rotation -= 1;
    //                         rotation %= 4;
    //                         currentEvent.Use();
    //                     }
    //                 }
    //                 break;
    //         }
    //
    //         for (int i = 0; i < targetGridCreator.transform.childCount; i++)
    //         {
    //             Handles.CubeHandleCap(0, targetGridCreator.transform.GetChild(i).position, Quaternion.identity, targetGridCreator.blockSize, EventType.Ignore);
    //         }
    //         
    //         if (GUI.changed)
    //             EditorUtility.SetDirty(base.target);
    //         SceneView.RepaintAll();
    //     }
    //
    //     private Vector3 GridToWorldPosition(Vector3Int position, int blocksPerTile = 1)
    //     {
    //         Vector3 worldPosition = default;
    //         worldPosition = (Vector3) position;
    //         return worldPosition;
    //     }
    //
    //     private Vector3Int WorldToGridPosition(Vector3 position, int blocksPerTile = 1)
    //     {
    //         Vector3Int gridPosition = default;
    //         gridPosition.x = Mathf.RoundToInt(position.x);
    //         gridPosition.y = Mathf.RoundToInt(position.y);
    //         gridPosition.z = Mathf.RoundToInt(position.z);
    //         return gridPosition;
    //     }
    //
    //     private void SpawnPreviewBlock()
    //     {
    //         if(previewBlock) DestroyImmediate(previewBlock);
    //         previewBlock = PrefabUtility.InstantiatePrefab(settings.blockSets[selectedSet].blocks[selectedBlock].gameObject) as GameObject;
    //         foreach (Collider collider in previewBlock.GetComponentsInChildren<Collider>())
    //         {
    //             collider.enabled = false;
    //         }
    //     }
    //
    //     private void RemoveBlock(GameObject block)
    //     {
    //         DestroyImmediate(block);
    //     }
    //
    //     private void PlaceBlock(Vector3Int gridPosition, Quaternion rotation, Transform parent)
    //     {
    //         BuildingBlock buildingBlock = PrefabUtility.InstantiatePrefab(settings.blockSets[selectedSet].blocks[selectedBlock]) as BuildingBlock;
    //         buildingBlock.transform.position = GridToWorldPosition(gridPosition);
    //         buildingBlock.transform.rotation = rotation;
    //         buildingBlock.transform.parent = parent;
    //     }
    //
    //     private static bool showLabels;
    //     
    //     public override void OnInspectorGUI()
    //     {
    //         base.OnInspectorGUI();
    //
    //         showLabels = GUILayout.Toggle(showLabels, "Show Labels");
    //         
    //         if (GUILayout.Button("Refesh"))
    //         {
    //             SetupReferences();
    //         }
    //         
    //         if (settings == null)
    //         {
    //             GUILayout.Label("Missing Settings", EditorStyles.boldLabel);
    //             return;
    //         }
    //         
    //         if(GUILayout.Button("Gather Connections"))
    //         {
    //             foreach (BuildingBlock buildingBlock in targetGridCreator.GetComponentsInChildren<BuildingBlock>())
    //             {
    //                 buildingBlock.GatherConnections();
    //             }
    //         }
    //         
    //         if (settings.blockSets != null)
    //         {
    //             Texture2D[] setIcons = settings.blockSets.Select(x => x.icon.texture).ToArray();
    //             int iconSize = 100;
    //             int setsPerRow = 5;
    //             
    //             GUILayout.Label("Sets");
    //             selectedSet = GUILayout.SelectionGrid(selectedSet, setIcons, setsPerRow, GUILayout.Width(iconSize * setsPerRow), GUILayout.Height(iconSize));
    //
    //             if (settings.blockSets[selectedSet].blocks != null && targetGridCreator?.tempIcon)
    //             {
    //                 Texture[] blockIcons = settings.blockSets[selectedSet].blocks.Select(x => targetGridCreator.tempIcon.texture).ToArray();
    //                 string[] blockNames = settings.blockSets[selectedSet].blocks.Select(x => x.name).ToArray();
    //                 int gridElementHeight = 30;
    //                 int blockCount = settings.blockSets[selectedSet].blocks.Length;
    //                 int iconsPerRow = 2;
    //                 int gridElementWidth = 500 / iconsPerRow;
    //                 int rows = Mathf.CeilToInt(blockCount / (float)iconsPerRow);
    //                 GUILayout.Label("Blocks");
    //                 selectedBlock = GUILayout.SelectionGrid(selectedBlock, blockNames, iconsPerRow, GUILayout.Width(gridElementWidth * iconsPerRow), GUILayout.Height(gridElementHeight * rows));
    //                 if (selectedBlock != previousSelectedBlock)
    //                 {
    //                     previousSelectedBlock = selectedBlock;
    //                     SpawnPreviewBlock();
    //                 }
    //             }
    //         }
    //     }
    //
    //     private static Vector3 GetPointOnPlane(Vector3 origin, Vector3 forward, Vector3 planeOrigin)
    //     {
    //         Vector3 normal = Vector3.up * -1f;
    //     
    //         float denominator = Vector3.Dot(forward, normal);
    //
    //         if (denominator > 0.00001f)
    //         {
    //             float distanceToPlane = Vector3.Dot(planeOrigin - origin, normal) / denominator;
    //             Vector3 intersectionPoint = origin + forward * distanceToPlane;
    //             return intersectionPoint;
    //         }
    //         
    //         return Vector3.zero;
    //     }
    // }
}