using Core;
using InventorySystem;
using Mirror;
using UnityEngine;

public class Spawner : NetworkBehaviour {
    public GameObject itemPrefab;

    [Command(requiresAuthority = false)]
    private void SpawnItemRpc(Item item, Vector3 position, Quaternion rotation) => SpawnItem(item, position, rotation);
    public void SpawnItem(Item item, Vector3 position, Quaternion rotation) {
        if (NetworkServer.active == false) {
            SpawnItemRpc(item, position, rotation);
            return;
        }
        
        var spawnedItem = Instantiate(itemPrefab, position, rotation);
        InventoryHandle inventoryHandle = GameManager.ItemManager.CreateInventory();
        spawnedItem.GetComponent<IItemContainer>().InventoryHandle = inventoryHandle;
        GameManager.ItemManager.Deposit(inventoryHandle, item.Handle);
        NetworkServer.Spawn(spawnedItem);
    }

    public GameObject Spawn(GameObject original, Transform parent = null) {
        GameObject spawned = Instantiate(original, parent);
        if (NetworkServer.active && spawned.TryGetComponent<NetworkIdentity>(out var networkObject) &&
            networkObject.didStart == false) {
            NetworkServer.Spawn(networkObject.gameObject);
        }

        return spawned;
    }

    public GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null) {
        var spawned = Instantiate(original, position, rotation, parent);
        if (spawned.TryGetComponent<NetworkIdentity>(out var networkObject) && networkObject.didStart == false) {
            NetworkServer.Spawn(networkObject.gameObject);
        }

        return spawned;
    }

    public T Spawn<T>(T original) where T : Component {
        var spawned = Instantiate(original);
        if (spawned.TryGetComponent<NetworkIdentity>(out var networkObject) && networkObject.didStart == false) {
            NetworkServer.Spawn(networkObject.gameObject);
        }

        return spawned;
    }

    public T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null)
        where T : Object {
        return Instantiate(original, position, rotation, parent);
    }

    public void Despawn(Actor target) {
        NetworkServer.Destroy(target.gameObject);
    }

    public void Despawn(NetworkBehaviour target) {
        NetworkServer.Destroy(target.gameObject);
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