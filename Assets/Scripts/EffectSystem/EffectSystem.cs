using UnityEngine;

namespace EffectSystem
{
    public class EffectManager : MonoBehaviour
    {
        public static void PlayEffect(EffectData effectData, GameObject target, GameObject instigator)
        {
            // PlayEffect(effectData.effects, target, instigator);
            foreach (var effect in effectData.effects)
            {
                var effectArgs = EffectArgs.Create<SpawnEffect.SpawnEffectArgs>();
                effectArgs.impactPosition = target.transform.position;
                effectArgs.impactRotation = target.transform.rotation;
                effectArgs.target = target;
                effectArgs.instigator = instigator;
                effect.DoEffect(effectArgs);
            }
        }

        public static void PlayEffect(EffectData effectData, GameObject target, GameObject instigator,
            Vector3 position = default, Quaternion rotation = default, AudioSource audioSource = default)
        {
            var rootEffectData = effectData;
            while (rootEffectData != null)
            {
                PlayEffect(rootEffectData.effects, target, instigator, position, rotation, audioSource);
                rootEffectData = rootEffectData.parentEffectData;
            }
        }

        public static void PlayEffect(Effect[] effects, GameObject target, GameObject instigator,
            Vector3 position = default, Quaternion rotation = default, AudioSource audioSource = default)
        {
            foreach (var effect in effects)
            {
                var effectArgs = EffectArgs.Create<SpawnEffect.SpawnEffectArgs>();
                effectArgs.impactPosition = position;
                effectArgs.impactRotation = rotation;
                effectArgs.target = target;
                effectArgs.instigator = instigator;
                effect.DoEffect(effectArgs);
            }
        }
    }
}