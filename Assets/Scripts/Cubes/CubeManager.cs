using System;
using System.Collections.Generic;
using Attributes;
using Core;
using EventManager;
using Mirror;
using UnityEngine;
using Attribute = Attributes.Attribute;

public class CubeManager : Actor {
    public Vector3 size = new(100, 1, 100);
    public GameObject blockPrefab;
    public Vector3 offset;
    public readonly SyncDictionary<Vector3, Block> blocks = new();
    public readonly Dictionary<Vector3, GameObject> spawnedBlocks = new();
    public HashSet<Vector3> destroyedBlocks = new();

    public override void OnStartServer() {
        blocks.OnChange += OnChange;

        for (var z = 0; z < size.z; z++)
        for (var y = 0; y < size.y; y++)
        for (var x = 0; x < size.x; x++) {
            var position = new Vector3Int(x, y, z);
            blocks[position] = Block.CreateDefault(position);
        }

        Events.AddListener(Flag.AttributeUpdated, handle, OnAttributeUpdated);
        GameManager.Attributes.SetupAttribute(handle, Attribute.Health, int.MaxValue);
    }

    private void OnAttributeUpdated(object origin, EventArgs eventargs) {
        var args = eventargs as AttributeEventArgs;
        DoDamage(WorldToGrid(args.position));
    }

    private Vector3 WorldToGrid(Vector3 worldPos) {
        return Vector3Int.RoundToInt(worldPos - offset);
    }


    private void OnChange(SyncIDictionary<Vector3, Block>.Operation op, Vector3 pos, Block block) {
        switch (op) {
            case SyncIDictionary<Vector3, Block>.Operation.OP_ADD: {
                var spawned = Instantiate(blockPrefab, pos + offset, default, transform);
                spawnedBlocks.Add(pos, spawned);
                spawned.GetComponent<Renderer>().material.color = Extensions.GetRandomColor();
                break;
            }
            case SyncIDictionary<Vector3, Block>.Operation.OP_SET: {
                if (blocks[pos].health <= 0) DestroyBlock(pos);

                break;
            }
            case SyncIDictionary<Vector3, Block>.Operation.OP_REMOVE: {
                Destroy(spawnedBlocks[pos]);
                spawnedBlocks.Remove(pos);
                destroyedBlocks.Add(pos);
                if (NetworkServer.active) SpawnNearbyBlocks(pos);

                break;
            }
        }
    }

    private void SpawnNearbyBlocks(Vector3 position) {
        var nearbyPositions = new List<Vector3Int> {
            new(-1, 0, 0),
            new(1, 0, 0),
            new(0, -1, 0),
            // new Vector3Int(0, 1, 0),
            new(0, 0, -1),
            new(0, 0, 1)
        };

        for (var i = 0; i < nearbyPositions.Count; i++) {
            if (destroyedBlocks.Contains(nearbyPositions[i] + position)) continue;
            if (blocks.ContainsKey(nearbyPositions[i] + position)) continue;
            blocks.Add(nearbyPositions[i] + position, Block.CreateDefault(nearbyPositions[i] + position));
        }
    }

    public void DoDamage(Vector3 position) {
        Debug.Log($"Trying to damage block: {position}");
        if (blocks.TryGetValue(position, out var block) == false)
            return;
        Debug.Log($"Damage block: {position}");
        block.health--;
        blocks[position] = block;
    }

    public void DestroyBlock(Vector3 position) {
        if (blocks.ContainsKey(position) == false)
            return;
        blocks.Remove(position);
    }
}

public struct Block {
    public Vector3 position;
    public int health;

    public static Block CreateDefault(Vector3 position) {
        return new Block {
            position = position,
            health = 1
        };
    }
}