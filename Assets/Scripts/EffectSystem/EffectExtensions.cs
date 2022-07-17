using System.Collections;
using System.Collections.Generic;
using EffectSystem;
using UnityEngine;

public static class EffectExtensions 
{
    public static void PlayEffect(this EffectData effectData, GameObject target, GameObject instigator)
    {
        if(effectData == null) return;
        EffectManager.PlayEffect(effectData, target, instigator);
    }

    public static void PlayEffect(this EffectData effectData, GameObject target, GameObject instigator, Vector3 position = default, Quaternion rotation = default, AudioSource audioSource = default)
    {
        if(effectData == null) return;
        EffectManager.PlayEffect(effectData, target, instigator, position, rotation, audioSource);
    }
}
