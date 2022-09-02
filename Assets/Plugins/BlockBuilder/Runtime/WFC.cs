using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBuilder
{
    public class WFC
    {
        private readonly BlockBuilderSettings settings;

        private class Tile
        {
            public int set;
            public List<int> possibleBlocks;
            public bool locked;
        }

        private Dictionary<Vector3Int, Tile> tiles;

        public WFC(BlockBuilderSettings settings, Dictionary<Vector3Int, int> blocks)
        {
            this.settings = settings;
            tiles = new Dictionary<Vector3Int, Tile>(blocks.Count);
            foreach (var block in blocks)
            {
                var possibleBlocks = new List<int>(settings.blockSets[block.Value].blockPrototypes.Count);
                for (var i = 1; i < settings.blockSets[block.Value].blockPrototypes.Count; i++)
                {
                    possibleBlocks.Add(i);
                }

                tiles[block.Key] = new Tile
                {
                    set = block.Value,
                    possibleBlocks = possibleBlocks
                };
            }
        }

        public Dictionary<Vector3Int, int> Run()
        {
            // Step 1: Propagate edge blocks
            // Check all blocks surrounding all the blocks. If no block, create air block and propagate
            var airTiles = new Dictionary<Vector3Int, Tile>();
            foreach (var tile in tiles)
            {
                void AddAirTile(Directions direction)
                {
                    Vector3Int neighborPosition = tile.Key + direction.ToVector3Int();
                    if (tiles.ContainsKey(neighborPosition) || airTiles.ContainsKey(neighborPosition)) return;
                    airTiles.Add(neighborPosition, new Tile
                    {
                        possibleBlocks = new List<int> {0},
                        locked = true,
                    });
                }
                
                AddAirTile(Directions.Right);
                AddAirTile(Directions.Back);
                AddAirTile(Directions.Left);
                AddAirTile(Directions.Forward);
                // AddAirTile(Directions.Down);
                // AddAirTile(Directions.Up);

                // for (int y = -1; y < 2; y++)
                // for (int x = -1; x < 2; x++)
                // for (int z = -1; z < 2; z++)
                // {
                //     // if (y != 0) continue;
                //     if ((x != 0 && z != 0 && y != 0) || x == z) continue;
                //     Vector3Int neighborPosition = tile.Key + new Vector3Int(x, y, z);
                //     if (tiles.ContainsKey(neighborPosition) || airTiles.ContainsKey(neighborPosition)) continue;
                //     airTiles.Add(neighborPosition, new Tile
                //     {
                //         possibleBlocks = new List<int> {0},
                //         locked = true,
                //     });
                // }
            }

            foreach (KeyValuePair<Vector3Int, Tile> airTile in airTiles)
            {
                tiles.Add(airTile.Key, airTile.Value);
                // if (Propagate(airTile.Key, airTile.Key) == false) return null;
                if (PropagateRipple(airTile.Key) == false) return null;
            }

            // Step 2: Collapse all blocks 
            while (GetWithLowestEntropy() is var (position, possibleBlocks) && possibleBlocks != int.MaxValue)
            {
                Collapse(position);
                if (PropagateRecursive(position, position) == false) return null;
            }

            // Step 3: Solution
            Dictionary<Vector3Int, int> solution = new Dictionary<Vector3Int, int>(tiles.Count);
            foreach (var tile in tiles)
            {
                if (tile.Value.possibleBlocks.Count == 0)
                {
                    throw new Exception("No possible solution!");
                    Debug.LogError("Bug here!");
                }
                solution[tile.Key] = tile.Value.possibleBlocks[0];
            }

            return solution;
        }

        private void Collapse(Vector3Int position)
        {
            var possibleBlocksCount = tiles[position].possibleBlocks.Count;
            var random = new System.Random();
            var randomNumber = random.Next(1, possibleBlocksCount);
            int blockIndex = possibleBlocksCount > 1 ? randomNumber : 0; // Dont pick the air block 
            var pickedBlock = tiles[position].possibleBlocks[blockIndex];
            tiles[position].possibleBlocks.Clear();
            tiles[position].possibleBlocks.Add(pickedBlock);
        }

        private (Vector3Int position, int possibleBlocks) GetWithLowestEntropy()
        {
            Vector3Int position = Vector3Int.zero;
            int possibleBlocks = int.MaxValue;
            foreach (var tile in tiles)
            {
                if (tile.Value.possibleBlocks.Count <= 1 || tile.Value.possibleBlocks.Count >= possibleBlocks) continue;
                possibleBlocks = tile.Value.possibleBlocks.Count;
                position = tile.Key;
            }

            return (position, possibleBlocks);
        }

        private bool PropagateRecursive(Vector3Int toPosition, Vector3Int fromPosition)
        {
            bool wasUpdated = toPosition == fromPosition; // Should only happen on the first tile
            Directions fromDirection = (fromPosition - toPosition).ToDirection();
            if (toPosition != fromPosition)
            {
                if (tiles[toPosition].locked) return true;
                int possibleCount = tiles[toPosition].possibleBlocks.Count;
                BuildingBlockSet.BlockFlags blockFlags = (BuildingBlockSet.BlockFlags) (1 << (byte) fromDirection);

                for (int i = tiles[toPosition].possibleBlocks.Count - 1; i >= 0; i--)
                for (int j = 0; j < tiles[fromPosition].possibleBlocks.Count; j++)
                {
                    Tile fromTile = tiles[fromPosition];
                    Tile toTile = tiles[toPosition];
                    BuildingBlockSet set = settings.blockSets[fromTile.set];
                    if (set.table[new Vector2Int(toTile.possibleBlocks[i], fromTile.possibleBlocks[j])].HasFlag(blockFlags))
                        break;

                    // If gotten to the end with no match, remove element
                    if (j == fromTile.possibleBlocks.Count - 1)
                        toTile.possibleBlocks.RemoveAt(i);
                }

                wasUpdated = possibleCount > tiles[toPosition].possibleBlocks.Count;

                if (tiles[toPosition].possibleBlocks.Count == 0)
                {
                    Debug.LogError("WFC failed. No possible solution");
                    return false;
                }
            }

            if (!wasUpdated) return true;

            // Propagate to all neighbors
            if (PropagateToNeighbor(Directions.Right) == false) return false;
            if (PropagateToNeighbor(Directions.Back) == false) return false;
            if (PropagateToNeighbor(Directions.Left) == false) return false;
            if (PropagateToNeighbor(Directions.Forward) == false) return false;
            if (PropagateToNeighbor(Directions.Up) == false) return false;
            if (PropagateToNeighbor(Directions.Down) == false) return false;

            bool PropagateToNeighbor(Directions directions)
            {
                // if (directions == fromDirection) return true;
                Vector3Int neighborTilePosition = toPosition + directions.ToVector3Int();
                if (neighborTilePosition == fromPosition) return true;
                if (!tiles.ContainsKey(neighborTilePosition)) return true;
                return PropagateRecursive(neighborTilePosition, toPosition);
            }

            return true;
        }

        private bool PropagateRipple(Vector3Int startTile)
        {
            HashSet<(Vector3Int fromTile, Vector3Int toTile)> openSet = new HashSet<(Vector3Int, Vector3Int)>();
            openSet.Add((startTile, startTile));

            while (openSet.Count > 0)
            {
                var newOpenSet = new HashSet<(Vector3Int, Vector3Int)>();
                foreach ((Vector3Int fromTile, Vector3Int toTile) in openSet)
                {
                    HashSet<(Vector3Int, Vector3Int)> openTileses = Propagate2(toTile, fromTile);
                    if (openTileses == null) continue;
                    foreach (var openTiles in openTileses)
                    {
                        newOpenSet.Add(openTiles);
                    }

                    // Dont forget about the has hset return value
                    // var function = new Func<HashSet<(Vector3Int, Vector3Int)>>(() => Propagate2(fromTile, toTile));
                    // tasks.Add(new TaskFactory().StartNew(function));
                }

                openSet = newOpenSet;

                // for (int i = 0; i < tasks.Count; i++)
                // tasks[i].Wait();

                // openSet.Clear();
                // for (int i = 0; i < tasks.Count; i++)
                // if(tasks[i].Result != null) openSet.UnionWith(tasks[i].Result);
            }

            return true;
        }

        private HashSet<(Vector3Int, Vector3Int)> Propagate2(Vector3Int toPosition, Vector3Int fromPosition)
        {
            bool wasUpdated = toPosition == fromPosition; // Should only happen on the first tile
            Directions fromDirection = (fromPosition - toPosition).ToDirection();
            if (toPosition != fromPosition)
            {
                if (tiles[toPosition].locked) return null;
                int possibleCount = tiles[toPosition].possibleBlocks.Count;
                BuildingBlockSet.BlockFlags blockFlags = (BuildingBlockSet.BlockFlags) (1 << (byte) fromDirection);

                for (int i = tiles[toPosition].possibleBlocks.Count - 1; i >= 0; i--)
                for (int j = 0; j < tiles[fromPosition].possibleBlocks.Count; j++)
                {
                    Tile fromTile = tiles[fromPosition];
                    Tile toTile = tiles[toPosition];
                    BuildingBlockSet set = settings.blockSets[fromTile.set];
                    if (set.table[new Vector2Int(toTile.possibleBlocks[i], fromTile.possibleBlocks[j])].HasFlag(blockFlags))
                    {
                        break;
                    }

                    // If gotten to the end with no match, remove element
                    if (j == fromTile.possibleBlocks.Count - 1)
                        toTile.possibleBlocks.RemoveAt(i);
                }

                wasUpdated = possibleCount > tiles[toPosition].possibleBlocks.Count;
            }

            if (!wasUpdated) return null;

            var openSet = new HashSet<(Vector3Int, Vector3Int)>();

            if (tiles.ContainsKey(toPosition + Directions.Right.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Right.ToVector3Int()));
            if (tiles.ContainsKey(toPosition + Directions.Back.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Back.ToVector3Int()));
            if (tiles.ContainsKey(toPosition + Directions.Left.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Left.ToVector3Int()));
            if (tiles.ContainsKey(toPosition + Directions.Forward.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Forward.ToVector3Int()));
            if (tiles.ContainsKey(toPosition + Directions.Up.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Up.ToVector3Int()));
            if (tiles.ContainsKey(toPosition + Directions.Down.ToVector3Int()))
                openSet.Add((toPosition, toPosition + Directions.Down.ToVector3Int()));

            return openSet;
        }
    }
}