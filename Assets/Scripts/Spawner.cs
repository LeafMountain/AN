using Core;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour {
    public NetworkObject itemPrefab;

    [Rpc(SendTo.Server)]
    private void SpawnItemRpc(Item item, Vector3 position, Quaternion rotation) => SpawnItem(item, position, rotation);
    public void SpawnItem(Item item, Vector3 position, Quaternion rotation) {
        if (IsServer == false) {
            SpawnItemRpc(item, position, rotation);
            return;
        }
        
        NetworkObject spawnedItem = Instantiate(itemPrefab, position, rotation);
        InventoryHandle inventoryHandle = GameManager.ItemManager.CreateInventory();
        spawnedItem.GetComponent<IItemContainer>().InventoryHandle = inventoryHandle;
        GameManager.ItemManager.Deposit(inventoryHandle, item.Handle);
        spawnedItem.Spawn();
    }

    public GameObject Spawn(GameObject original, Transform parent = null) {
        GameObject spawned = Instantiate(original, parent);
        if (NetworkManager.Singleton.IsServer && spawned.TryGetComponent<NetworkObject>(out var networkObject) &&
            networkObject.IsSpawned == false) {
            networkObject.Spawn();
        }

        return spawned;
    }

    public GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null) {
        var spawned = Instantiate(original, position, rotation, parent);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false) {
            networkObject.Spawn();
        }

        return spawned;
    }

    public T Spawn<T>(T original) where T : Component {
        var spawned = Instantiate(original);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false) {
            networkObject.Spawn();
        }

        return spawned;
    }

    public T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null)
        where T : Object {
        return Instantiate(original, position, rotation, parent);
    }

    public void Despawn(Actor target) {
        target.NetworkObject.Despawn();
    }

    public void Despawn(NetworkBehaviour target) {
        target.NetworkObject.Despawn();
    }

    public void Despawn(NetworkObject target) {
        target.Despawn();
    }

    public void Despawn(GameObject gameObject, float delay = 0) {
        if (delay == 0) {
            Destroy(gameObject);
        }
        else {
            Destroy(gameObject, delay);
        }
    }
}