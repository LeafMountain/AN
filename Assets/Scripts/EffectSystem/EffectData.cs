using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace EffectSystem
{
    [CreateAssetMenu(menuName = "AN/EffectData")]
    public class EffectData : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        public Effect[] effects = Array.Empty<Effect>();
    }
}
