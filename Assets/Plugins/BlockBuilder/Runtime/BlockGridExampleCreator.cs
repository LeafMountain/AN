using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlockBuilder
{
    public class BlockGridExampleCreator : MonoBehaviour
    {
        public float blockSize = 1f;
        public double tolerance = .1;
        public int decimals = 4;
        public BuildingBlockSet blockSet;
        public GameObject airBlock;

        [Header("Gizmos")] public bool showXSide;
        public bool showYSide;
        public bool showZSide;

        private static List<Vector3>[] GetSideVertices(GameObject block, double blockSize, double tolerance = .001f)
        {
            double halfBlockSize = blockSize * .5;

            var xSideVertsPos = new HashSet<Vector3>();
            var xSideVertsNeg = new HashSet<Vector3>();
            var zSideVertsPos = new HashSet<Vector3>();
            var zSideVertsNeg = new HashSet<Vector3>();
            var ySideVertsPos = new HashSet<Vector3>();
            var ySideVertsNeg = new HashSet<Vector3>();

            MeshFilter meshFilter = block.GetComponent<MeshFilter>();
            if(meshFilter == null) meshFilter = block.GetComponentInChildren<MeshFilter>();
            if (meshFilter)
            {
                Mesh sharedMesh = meshFilter.sharedMesh;
                for (var i = 0; i < sharedMesh.vertices.Length; i++)
                {
                    // X
                    if (Math.Abs(sharedMesh.vertices[i].x - halfBlockSize) < tolerance)
                        xSideVertsPos.Add(sharedMesh.vertices[i]);
                    if (Math.Abs(sharedMesh.vertices[i].x - (-halfBlockSize)) < tolerance)
                        xSideVertsNeg.Add(sharedMesh.vertices[i]);

                    // Z
                    if (Math.Abs(sharedMesh.vertices[i].z - halfBlockSize) < tolerance)
                        zSideVertsPos.Add(sharedMesh.vertices[i]);
                    if (Math.Abs(sharedMesh.vertices[i].z - (-halfBlockSize)) < tolerance)
                        zSideVertsNeg.Add(sharedMesh.vertices[i]);

                    // Y
                    if (Math.Abs(sharedMesh.vertices[i].y - halfBlockSize) < tolerance)
                        ySideVertsPos.Add(sharedMesh.vertices[i]);
                    if (Math.Abs(sharedMesh.vertices[i].y - (-halfBlockSize)) < tolerance)
                        ySideVertsNeg.Add(sharedMesh.vertices[i]);
                }
            }

            return new[] { xSideVertsPos.ToList(), zSideVertsNeg.ToList(), xSideVertsNeg.ToList(), zSideVertsPos.ToList(), ySideVertsPos.ToList(), ySideVertsNeg.ToList() };
        }

        private void OnDrawGizmosSelected()
        {
            if (!showXSide && !showYSide && !showZSide) return;

            GameObject[] blocks = GetBlocks();

            foreach (var block in blocks)
            {
                if (block.gameObject.activeInHierarchy == false) continue;
                Gizmos.color = new Color(.9f, .7f, .2f);
                Gizmos.DrawWireCube(block.transform.position, Vector3.one * blockSize);

                var sideVertices = GetSideVertices(block, blockSize, tolerance);

                if (showXSide)
                {
                    DrawVerts(block, sideVertices[(int)Directions.Right], Color.red);
                    DrawVerts(block, sideVertices[(int)Directions.Left], Color.red);
                }

                if (showZSide)
                {
                    DrawVerts(block, sideVertices[(int)Directions.Forward], Color.blue);
                    DrawVerts(block, sideVertices[(int)Directions.Back], Color.blue);
                }

                if (showYSide)
                {
                    DrawVerts(block, sideVertices[(int)Directions.Up], Color.green);
                    DrawVerts(block, sideVertices[(int)Directions.Down], Color.green);
                }

                void DrawVerts(GameObject block, List<Vector3> verts, Color color)
                {
                    Gizmos.color = color;
                    foreach (var vert in verts)
                        Gizmos.DrawSphere(block.transform.position + vert, .05f);
                }
            }
        }

        private static bool Fits(List<Vector3> sideA, List<Vector3> sideB)
        {
            if (sideA.Count != sideB.Count) return false;
            var instancedA = sideA.OrderBy(x => (x - Vector3.up * 10).magnitude).ToList();
            var instancedB = FlipZ(sideB.OrderBy(x => (x - Vector3.up * 10).magnitude).ToList());
            for (int i = 0; i < instancedA.Count; i++)
                if (instancedA[i] != instancedB[i])
                    return false;
            return true;
        }

        private static List<Vector3> FlipZ(IReadOnlyCollection<Vector3> points)
        {
            var flippedPoints = new List<Vector3>(points);
            for (int i = 0; i < flippedPoints.Count; i++)
                flippedPoints[i] = new Vector3(flippedPoints[i].x, flippedPoints[i].y, -flippedPoints[i].z);
            return flippedPoints;
        }

        private static List<Vector3> Rotate(IReadOnlyCollection<Vector3> points, int rotations)
        {
            List<Vector3> rotatedPoints = new List<Vector3>(points);
            if (rotations == 4)
            {
                rotations = 1;
                for (int j = 0; j < points.Count; j++)
                    rotatedPoints[j] = new Vector3(rotatedPoints[j].x, -rotatedPoints[j].z, rotatedPoints[j].y);
            }
            else if (rotations == 5)
            {
                rotations = 3;
                for (int j = 0; j < points.Count; j++)
                    rotatedPoints[j] = new Vector3(rotatedPoints[j].x, -rotatedPoints[j].z, rotatedPoints[j].y);
            }
            for (int i = 0; i < rotations; i++)
            for (int j = 0; j < points.Count; j++)
                rotatedPoints[j] = new Vector3(-rotatedPoints[j].z, rotatedPoints[j].y, rotatedPoints[j].x);
            return rotatedPoints;
        }

        private GameObject[] GetBlocks()
        {
            List<GameObject> blocks = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                blocks.Add(transform.GetChild(i).gameObject);
            }

            return blocks.ToArray();
        }

        Directions GetRotatedDirection(Directions direction, int rotation) => direction.Rotate(rotation);

        [ContextMenu("Setup Blocks")]
        public void SetupBlocks()
        {
            blockSet.blockPrototypes.Clear();

            // Always add air block
            Vector3 airConnection = new Vector3(0f, 0f, -1f);
            blockSet.blockPrototypes.Add(new BuildingBlockSet.BuildingBlock
            {
                blockPrefab = airBlock,
                rotation = 0,
                sideIDs = new[] { airConnection, airConnection, airConnection, airConnection, airConnection, airConnection, }
            });

            var blocks = GetBlocks();
            foreach (var block in blocks)
            {
                const int rotations = 4;
                for (int i = 0; i < rotations; i++)
                {
                    if (block.activeInHierarchy == false) continue;
                    if (blockSet.blockPrototypes.Exists(x => x.blockPrefab == block && x.rotation == i)) continue;

                    var sideVerts = GetSideVertices(block, blockSize, tolerance);
                    var rightHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Right, i)], GetRotatedDirection(Directions.Right, i), decimals);
                    var backHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Back, i)], GetRotatedDirection(Directions.Back, i), decimals);
                    var leftHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Left, i)], GetRotatedDirection(Directions.Left, i), decimals);
                    var forwardHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Forward, i)], GetRotatedDirection(Directions.Forward, i), decimals);
                    var upHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Up, i)], Directions.Down, decimals);
                    var downHash = GetSideHash(sideVerts[(int)GetRotatedDirection(Directions.Down, i)], Directions.Up, decimals);

                    blockSet.blockPrototypes.Add(new BuildingBlockSet.BuildingBlock
                    {
                        blockPrefab = PrefabUtility.GetCorrespondingObjectFromSource(block),
                        rotation = i,
                        sideIDs = new[] { rightHash, backHash, leftHash, forwardHash, upHash, downHash }
                    });
                }
            }
            
            blockSet.SaveToFile(); 

            EditorUtility.SetDirty(blockSet);
            AssetDatabase.SaveAssetIfDirty(blockSet);
            AssetDatabase.Refresh();
        }

        // I'm just expecting the results to be different enough. Might overlap
        private static Vector3 GetSideHash(IReadOnlyCollection<Vector3> sideVerts, Directions side, int decimals)
        {
            if (sideVerts == null || sideVerts.Count == 0) return new Vector3(0f, 0f, 1f);
            Vector3 sideSum = default;
            var rotatedPoints = Rotate(sideVerts, (int)side);
            rotatedPoints.ForEach(point => sideSum += point);
            return new Vector3(
                (float)Math.Round(sideSum.x, decimals, MidpointRounding.AwayFromZero),
                (float)Math.Round(sideSum.y, decimals, MidpointRounding.AwayFromZero),
                (float)Math.Round(sideSum.z, decimals, MidpointRounding.AwayFromZero));
        }

        [ContextMenu("Grid Blocks")]
        private void GridBlocks()
        {
            var blocks = GetBlocks();
            const int gridWidth = 4;
            const float cellSize = 2.5f;
            for (int i = 0; i < blocks.Length; i++)
            {
                int col = Mathf.FloorToInt(i / gridWidth);
                int row = i % gridWidth;
                
                blocks[i].transform.localPosition = Vector3.right * row * cellSize;
                blocks[i].transform.localPosition += Vector3.forward * col * cellSize;
            }
        }
    }
}