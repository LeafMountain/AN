using System.Collections.Generic;
using Core;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class WorldInventory : NetworkBehaviour, IItemContainer
    {
        private readonly NetworkList<ItemAccessor> items = new();
        private readonly List<GameObject> spawnedVisuals = new();

        public List<Transform> slots = new();

        private void Awake()
        {
            items.OnListChanged += OnItemsUpdated;
        }

        private void OnItemsUpdated(NetworkListEvent<ItemAccessor> changeevent)
        {
            SpawnVisuals();
            transform.DOShakeRotation(.1f, 5f);
        }

        public void DepositImplementation(ItemAccessor itemAccessId)
        {
            items.Add(itemAccessId);
        }

        public void WithdrawImplementation(ItemAccessor itemAccessId)
        {
            items.Remove(itemAccessId);
        }

        private void SpawnVisuals()
        {
            DestroyVisuals();

            for (int i = 0; i < items.Count; i++)
            {
                ItemAccessor itemAccessor = items[i];
                Item? item = itemAccessor.GetItem();
                Transform slot = slots.Count > i ? slots[i] : transform;
                if (item.HasValue)
                {
                    ItemData itemData = GameManager.Database.GetItem(item.Value.databaseId);
                    itemData.graphics.InstantiateAsync(slot.position, slot.rotation, slot).Completed += handle =>
                    {
                        handle.Result.transform.localScale = Vector3.one;
                        spawnedVisuals.Add(handle.Result);
                    };
                }
            }
        }

        private void DestroyVisuals()
        {
            if (spawnedVisuals.Count != 0)
            {
                for (int i = spawnedVisuals.Count - 1; i >= 0; i--)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                    {
                        DestroyImmediate(spawnedVisuals[i]);
                    }
                    else
#endif
                    {
                        Destroy(spawnedVisuals[i]);
                    }

                    spawnedVisuals.RemoveAt(i);
                }
            }
        }
    }
}