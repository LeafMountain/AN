using System;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace EffectSystem
{
    [Serializable]
    public abstract class Effect
    {
        public enum Mode
        {
            Target,
            Instigator,
            Impact
        }

        [SerializeField] protected Mode spawnMode;

        public virtual void DoEffect<T>(T extraArgs) where T : EffectArgs
        {
        }
    }

    public class EffectArgs
    {
        public GameObject target;
        public GameObject instigator;

        protected EffectArgs()
        {
        }

        public static T Create<T>() where T : EffectArgs, new()
        {
            return new T();
        }

        protected virtual void Reset()
        {
            target = null;
            instigator = null;
        }
    }

    [Serializable]
    public class SpawnEffect : Effect
    {
        public class SpawnEffectArgs : EffectArgs
        {
            public Vector3 impactPosition;
            public Vector3 impactNormal;
            public Quaternion impactRotation;

            protected override void Reset()
            {
                base.Reset();
                impactPosition = default;
                impactRotation = default;
                impactNormal = default;
            }
        }

        [SerializeField] private GameObject gameObject;
        [SerializeField] private bool autoDestroy;

        [SerializeField, ShowIf(nameof(autoDestroy))]
        private float destroyDelayDuration;

        public override void DoEffect<T>(T extraArgs)
        {
            var spawnArgs = extraArgs as SpawnEffectArgs;
            GameObject spawned = null;

            switch (spawnMode)
            {
                case Mode.Target:
                    spawned = Object.Instantiate(gameObject, extraArgs.target.transform.position,
                        extraArgs.target.transform.rotation);
                    break;

                case Mode.Instigator:
                    spawned = Object.Instantiate(gameObject, extraArgs.instigator.transform.position,
                        extraArgs.instigator.transform.rotation);
                    break;

                case Mode.Impact:
                    spawned = Object.Instantiate(gameObject, spawnArgs.impactPosition, spawnArgs.impactRotation);
                    break;
            }


            if (autoDestroy)
            {
                float destroyDuration = destroyDelayDuration;
                if (destroyDelayDuration <= 0)
                {
                    if (gameObject.TryGetComponent<ParticleSystem>(out var particleSystem))
                        destroyDuration = particleSystem.main.duration;
                }

                GameManager.Despawn(spawned, destroyDuration);
            }
        }
    }

    [Serializable]
    public class CameraEffect : Effect
    {
        [SerializeField] private float shakeTime;
        [SerializeField] private float shakeAmplitude;

        public override void DoEffect<T>(T extraArgs)
        {
            CameraController.Shake(shakeTime, shakeAmplitude);
        }
    }

    [Serializable]
    public class TweenEffect : Effect
    {
        [SerializeField] private float duration = 1;

        [SerializeField] private float strength = 1;
        // [SerializeField]
        // private Vector3 vector3;

        public static Dictionary<GameObject, Tweener> activeTweeners = new();

        public override void DoEffect<T>(T extraArgs)
        {
            switch (spawnMode)
            {
                case Mode.Target:
                    if (activeTweeners.TryGetValue(extraArgs.target, out var tweener))
                    {
                        tweener.Rewind(false);
                        tweener.Kill();
                        activeTweeners.Remove(extraArgs.target);
                    }

                    activeTweeners[extraArgs.target] =
                        extraArgs.target.transform.DOPunchScale(Vector3.one * strength, duration);
                    break;

                case Mode.Instigator:
                    if (activeTweeners.TryGetValue(extraArgs.instigator, out tweener))
                    {
                        tweener.Rewind(false);
                        tweener.Kill();
                        activeTweeners.Remove(extraArgs.instigator);
                    }

                    activeTweeners[extraArgs.instigator] =
                        extraArgs.instigator.transform.DOPunchScale(Vector3.one * strength, duration);
                    break;

                case Mode.Impact:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}