using Core;
using InventorySystem;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public NetworkObject itemPrefab;

    public void SpawnItem(Item item, Vector3 position, Quaternion rotation)
    {
        NetworkObject spawnedItem = Instantiate(itemPrefab, position, rotation);
        // spawnedItem.GetComponent<Storeable>().SetItemData(item);
        GameManager.ItemManager.Deposit(spawnedItem.GetComponent<IItemContainer>(), item.accessId);
        spawnedItem.Spawn();
    }

    public GameObject Spawn(GameObject original, Transform parent = null)
    {
        GameObject spawned = Instantiate(original, parent);
        if (NetworkManager.Singleton.IsServer && spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false)
        {
            networkObject.Spawn();
        }

        return spawned;
    }
    
    public GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var spawned = Instantiate(original, position, rotation, parent);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false)
        {
            networkObject.Spawn();
        }

        return spawned;
    }

    public T Spawn<T>(T original) where T : Component
    {
        var spawned = Instantiate(original);
        if (spawned.TryGetComponent<NetworkObject>(out var networkObject) && networkObject.IsSpawned == false)
        {
            networkObject.Spawn();
        }

        return spawned;
    }

    public T Spawn<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null)
        where T : MonoBehaviour
    {
        return Instantiate(original, position, rotation, parent);
    }
    
    public void Despawn(Actor target)
    {
        target.NetworkObject.Despawn();
    }
    
    public void Despawn(NetworkBehaviour target)
    {
        target.NetworkObject.Despawn();
    }
    
    public void Despawn(NetworkObject target)
    {
        target.Despawn();
    }
    
    public void Despawn(GameObject gameObject, float delay = 0)
    {
        if (delay == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, delay);
        }
    }
}