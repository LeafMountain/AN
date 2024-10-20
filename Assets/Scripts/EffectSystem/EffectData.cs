using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EffectSystem
{
    [CreateAssetMenu(menuName = "AN/EffectData")]
    public class EffectData : SerializedScriptableObject
    {
        [Tooltip("Will be triggered alongside this effect")]
        public EffectData parentEffectData;
        
        [NonSerialized, OdinSerialize]
        public Effect[] effects = Array.Empty<Effect>();
    }
}
