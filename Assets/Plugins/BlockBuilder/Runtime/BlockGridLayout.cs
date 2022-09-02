using UnityEngine;

[CreateAssetMenu(menuName = "BlockBuilder/Block Grid Layout")]
public class BlockGridLayout : ScriptableObject
{
    // [y], [x, z]
    public int[][,] layout;

    public Vector3Int gridSize;

    public void SetSize(Vector3Int gridSize)
    {
        layout = new int[gridSize.y][,];
        for (int y = 0; y < layout.Length; y++)
        {
            layout[y] = new int[gridSize.x, gridSize.z];
        }
    }

    // [Button("Set Size")]
    public void SetSize() => SetSize(gridSize);

    public int fillLayerY = 0;

    // [Button("Fill Floor")]
    public void FillFloor()
    {
        for (int z = 0; z < gridSize.z; z++)
            for (int x = 0; x < gridSize.x; x++)
            {
                layout[fillLayerY][x, z] = 0;
            }
    }
}