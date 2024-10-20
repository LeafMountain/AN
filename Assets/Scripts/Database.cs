using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core 
{
    [CreateAssetMenu(menuName = "AN/Database")]
    public class Database : SerializedScriptableObject
    {
        public ItemData[] items;

        [Button]
        public void Init()
        {
            foreach (ItemData itemData in items)
            {
                itemData.id = StringToID(itemData.slug);
            }
        }

        public int StringToID(string slug) => Animator.StringToHash(slug);
        public ItemData GetItem(string slug) => GetItem(StringToID(slug));
        public ItemData GetItem(int id) => items.FirstOrDefault(x => x.id == id);
    }

    [Serializable]
    public class ItemData
    {
        [ReadOnly] public int id;
        public string slug;
        public string name;
        public AssetReference graphics;
    }
}