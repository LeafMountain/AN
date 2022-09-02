using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace BlockBuilder
{
    [CreateAssetMenu(menuName = "BlockBuilder/Block Set")]
    public class BuildingBlockSet : ScriptableObject
    {
        public Sprite icon;
        public int blocksPerTile = 1;

        [Serializable]
        public class ForbiddenConnection
        {
            public GameObject blockPrefab;
            public GameObject incompatibleBlock;
        }

        public ForbiddenConnection[] forbiddenConnections;

        [Serializable]
        public class BuildingBlock
        {
            public GameObject blockPrefab;
            public int rotation;
            public Vector3[] sideIDs;

            public static BuildingBlock Air(GameObject airBlockPrefab)
            {
                return new BuildingBlock
                {
                    blockPrefab = airBlockPrefab,
                    sideIDs = new[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, }
                };
            }

            public bool Equals(BuildingBlock other)
            {
                return Equals(blockPrefab, other.blockPrefab) && rotation == other.rotation;
            }

            // Rider thought I needed this. I'll just leave it for now
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((blockPrefab != null ? blockPrefab.GetHashCode() : 0) * 397) ^ rotation;
                }
            }
        }

        // Block data
        public List<BuildingBlock> blockPrototypes = new List<BuildingBlock>();

        [Flags]
        public enum BlockFlags : byte
        {
            Right = 1 << Directions.Right,
            Back = 1 << Directions.Back,
            Left = 1 << Directions.Left,
            Forward = 1 << Directions.Forward,
            Up = 1 << Directions.Up,
            Down = 1 << Directions.Down,
        }

        public Dictionary<Vector2, BlockFlags> table = new Dictionary<Vector2, BlockFlags>();

        public void SaveToFile()
        {
            table.Clear();

            for (int i = 0; i < blockPrototypes.Count; i++)
            for (int j = 0; j < blockPrototypes.Count; j++)
            {
                BlockFlags connectionBits = 0;

                BlockFlags ConnectionBits(Directions direction)
                {
                    Vector3 sideID = blockPrototypes[i].sideIDs[(int)direction];
                    Vector3 neighborSideID = blockPrototypes[j].sideIDs[(int)direction.Opposite()];
                    // if ((int) direction > 3)
                        // neighborSideID.y = -neighborSideID.y;
                    neighborSideID.z = -neighborSideID.z;
                    if (sideID == neighborSideID) return connectionBits |= (BlockFlags)(1 << (int)direction);
                    return 0;
                }

                connectionBits |= ConnectionBits(Directions.Right);
                connectionBits |= ConnectionBits(Directions.Back);
                connectionBits |= ConnectionBits(Directions.Left);
                connectionBits |= ConnectionBits(Directions.Forward);
                connectionBits |= ConnectionBits(Directions.Up);
                connectionBits |= ConnectionBits(Directions.Down);

                table[new Vector2Int(i, j)] = connectionBits;
            }

            // Forbidden connections
            for (int i = 0; i < forbiddenConnections.Length; i++)
            {
                List<int> blockIDs = new List<int>(4);

                for (var j = 0; j < blockPrototypes.Count; j++)
                {
                    if (blockPrototypes[j].blockPrefab == forbiddenConnections[i].blockPrefab)
                        blockIDs.Add(j);
                }

                // Unset bit for every 90 degree block
                for (var k = 0; k < blockIDs.Count; k++)
                {
                    table[new Vector2Int(blockIDs[k], blockIDs[(k + 1) % 4])] &= ~(BlockFlags)k;
                    // table[(blockIDs[k], blockIDs[(k + 3) % 4])] &= ~(BlockFlags)k;
                }
            }

            // string serializedBlocks = JsonConvert.SerializeObject(blockPrototypes);
            // File.WriteAllText($"{Application.dataPath}/BlockSets/BlockPrototypes_{name}.json", serializedBlocks);

            // JsonSerializerSettings settings = new JsonSerializerSettings();
            // settings.Converters.Add(new TupleConverter<int, int>());
            string serializedTable = JsonConvert.SerializeObject(table);
            File.WriteAllText($"{Application.dataPath}/BlockSets/BlockTable_{name}.json", serializedTable);
        }

        public void LoadFromFile()
        {
            // string jsonBlocks= File.ReadAllText($"{Application.dataPath}/BlockSets/BlockPrototypes_{name}.json");
            // blockPrototypes = JsonConvert.DeserializeObject<List<BuildingBlock>>(jsonBlocks);

            // JsonSerializerSettings settings = new JsonSerializerSettings();
            // settings.Converters.Add(new TupleConverter<int, int>());
            string jsonTable = File.ReadAllText($"{Application.dataPath}/BlockSets/BlockTable_{name}.json");
            table = JsonConvert.DeserializeObject<Dictionary<Vector2, BlockFlags>>(jsonTable);
        }
    }
}