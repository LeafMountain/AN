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
}