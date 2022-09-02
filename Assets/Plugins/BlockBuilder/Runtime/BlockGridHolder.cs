using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BlockBuilder
{
    public class BlockGridHolder : MonoBehaviour
    {
        [Serializable]
        public struct PlacedBlock
        {
            public Vector3Int position;
            public int set;
        }

        public BlockBuilderSettings settings;
        public List<PlacedBlock> serializedPlacedBlocks = new List<PlacedBlock>();
        public readonly Dictionary<Vector3Int, int> placedBlocks = new Dictionary<Vector3Int, int>(); // want to keep this simple. Can be synced over the network
        public Color gizmoGridColor = new Color(1f, 1f, 1f, .5f);
        public int blockSize = 2;
        
        public bool drawGizmos = false;

        public void PlaceBlock(Vector3 position, int blockSetID, int blocksPerTile = -1)
        {
            var worldToGridPosition = WorldToGridPosition(this, position, -1, settings.blockSets[blockSetID].blocksPerTile);
            PlaceBlock(worldToGridPosition, blockSetID, blocksPerTile);
        }
        
        public void PlaceBlock(Vector3Int position, int blockSetID, int blocksPerTile = -1)
        {
            if (blocksPerTile == -1)
            {
                blocksPerTile = settings.blockSets[blockSetID].blocksPerTile;
            }
            
            for (int z = position.z * blocksPerTile; z < position.z * blocksPerTile + blocksPerTile; z++)
            for (int y = position.y * blocksPerTile; y < position.y * blocksPerTile + blocksPerTile; y++)
            for (int x = position.x * blocksPerTile; x < position.x * blocksPerTile + blocksPerTile; x++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, z);
                if (placedBlocks.ContainsKey(gridPosition) == false) placedBlocks.Add(gridPosition, 0);
                placedBlocks[gridPosition] = blockSetID;

                var index = serializedPlacedBlocks.FindIndex(x => x.position == gridPosition);
                if (index != -1 && blockSetID == -1)
                {
                    serializedPlacedBlocks.RemoveAt(index);
                }
                else
                {
                    PlacedBlock placedBlock = new PlacedBlock();
                    placedBlock.position = gridPosition;
                    placedBlock.set = blockSetID;
                    if (index == -1) serializedPlacedBlocks.Add(placedBlock);
                    else serializedPlacedBlocks[index] = placedBlock;
                }
            }
        }

        public void RemoveBlock(Vector3 position, int blocksPerTile = 1)
        {
            var gridPosition = WorldToGridPosition(this, position);
            RemoveBlock(gridPosition, blocksPerTile);
        }
        
        public void RemoveBlock(Vector3Int position, int blocksPerTile = 1)
        {
            for (int z = position.z * blocksPerTile; z < position.z * blocksPerTile + blocksPerTile; z++)
            for (int y = position.y * blocksPerTile; y < position.y * blocksPerTile + blocksPerTile; y++)
            for (int x = position.x * blocksPerTile; x < position.x * blocksPerTile + blocksPerTile; x++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, z);
                if (placedBlocks.ContainsKey(gridPosition)) placedBlocks.Remove(gridPosition);
                var index = serializedPlacedBlocks.FindIndex(x => x.position == gridPosition);
                if (index != -1)
                {
                    serializedPlacedBlocks.RemoveAt(index);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(drawGizmos == false) return;
            Gizmos.color = gizmoGridColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            for (var i = 0; i < serializedPlacedBlocks.Count; i++)
            {
                Gizmos.DrawCube(GridToWorldPosition(this, serializedPlacedBlocks[i].position), Vector3.one * blockSize);
            }
        }

        [ContextMenu("Run Algorithm")]
        public void Run()
        {
            bool async = false;
            
            placedBlocks.Clear();
            for (var i = 0; i < serializedPlacedBlocks.Count; i++)
                placedBlocks[serializedPlacedBlocks[i].position] = serializedPlacedBlocks[i].set;

            // Load block sets
            foreach (var settingsBlockSet in settings.blockSets)
            {
                settingsBlockSet.SaveToFile();
            }
            // settingsBlockSet.LoadFromFile(); 

            // Spawn blocks
            var wfc = new WFC(settings, placedBlocks);
            Dictionary<Vector3Int, int> solution = null;
            var handle = new Task<Dictionary<Vector3Int, int>>(wfc.Run);
            if (async)
            {
                handle.Start();
            }
            else
            {
                handle.RunSynchronously();
            }

            handle.Wait();
            solution = handle.Result;

            // Debug.Log(solution.Count);

            if (true)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }

                foreach (var tileSolution in solution)
                {
                    // if (tileSolution.Value == 0) continue;
                    var blockPrototype = settings.blockSets[0].blockPrototypes[tileSolution.Value];
                    Vector3 tileWorldPosition = GridToWorldPosition(this, tileSolution.Key);
                    Instantiate(blockPrototype.blockPrefab, tileWorldPosition, Quaternion.Euler(0f, 360f - 90f * blockPrototype.rotation, 0f), transform);
                }
            }
        }

        public static Vector3Int WorldToGridPosition(BlockGridHolder gridHolder, Vector3 position, int blockSize = -1, int blocksPerTile = 2)
        {
            if (blockSize == -1)
                blockSize = gridHolder.blockSize;
            
            float sizePerTile = blockSize * blocksPerTile;
            Vector3Int gridPosition = default;
            position -= gridHolder.transform.position;
            gridPosition.x = Mathf.FloorToInt(position.x / sizePerTile);
            gridPosition.y = Mathf.FloorToInt(position.y / sizePerTile);
            gridPosition.z = Mathf.FloorToInt(position.z / sizePerTile);
            return gridPosition;
        }

        public static Vector3 GridToWorldPosition(BlockGridHolder gridHolder, Vector3Int gridPosition, int blockSize = -1, int blocksPerTile = 2)
        {
            if (blockSize == -1)
                blockSize = gridHolder.blockSize;
            
            Vector3 worldPosition = gridHolder.transform.position;
            worldPosition += new Vector3(gridPosition.x * blockSize, gridPosition.y * blockSize, gridPosition.z * blockSize);
            worldPosition += Vector3.one * (blockSize * .5f); // Offset
            return worldPosition;
        }
    }
}