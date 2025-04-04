using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace EffectSystem {
    [Serializable]
    public abstract class Effect {
        public enum Mode {
            Target,
            Instigator,
            Impact
        }

        [SerializeField] protected Mode spawnMode;
        [SerializeField] protected string pointType;

        public virtual void DoEffect<T>(T extraArgs) where T : EffectArgs { }

        public virtual (Vector3 point, Quaternion rotation) GetLocation(EffectArgs extraArgs) {
            // switch (spawnMode)
            // {
            //     case Mode.Target:
            //     {
            //         var actor = extraArgs.target.GetComponent<Actor>();
            //         return actor.GetPointAndRotation(pointType);
            //     }
            //     case Mode.Instigator:
            //     {
            //         var actor = extraArgs.instigator.GetComponent<Actor>();
            //         return actor.GetPointAndRotation(pointType);
            //     }
            //     case Mode.Impact:
            //         return (extraArgs.target.GetComponent<DamageReceiver>().lastHitPoint,
            //             Quaternion.LookRotation(extraArgs.target.GetComponent<DamageReceiver>().lastHitNormal) *
            //             Quaternion.Euler(Vector3.right * 90f));
            // }

            return (default, default);
        }
    }

    public class EffectArgs {
        public GameObject instigator;
        public GameObject target;

        protected EffectArgs() { }

        public static T Create<T>() where T : EffectArgs, new() {
            return new T();
        }

        protected virtual void Reset() {
            target = null;
            instigator = null;
        }
    }

    [Serializable]
    public class SpawnEffect : Effect {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private VisualEffectAsset visualEffect;
        [SerializeField] private bool autoDestroy;

        [SerializeField] [ShowIf(nameof(autoDestroy))]
        private float destroyDelayDuration;

        public override void DoEffect<T>(T extraArgs) {
            var spawnArgs = extraArgs as SpawnEffectArgs;
            GameObject spawned = null;
            if (visualEffect != null) {
                spawned = new GameObject(this.visualEffect.name);
                var visualEffect = spawned.AddComponent<VisualEffect>();
                visualEffect.visualEffectAsset = this.visualEffect;
            }

            // spawned = GameManager.Spawner.Spawn(gameObject);
            switch (spawnMode) {
                case Mode.Target: {
                    var (point, rotation) = GetLocation(extraArgs);
                    spawned.transform.position = point;
                    spawned.transform.rotation = rotation;
                    break;
                }

                case Mode.Instigator: {
                    var (point, rotation) = GetLocation(extraArgs);
                    spawned.transform.position = point;
                    spawned.transform.rotation = rotation;
                    // spawned.transform.position = extraArgs.instigator.transform.position;
                    // spawned.transform.rotation = extraArgs.instigator.transform.rotation;
                    break;
                }

                case Mode.Impact:
                    spawned.transform.position = spawnArgs.impactPosition;
                    spawned.transform.rotation = spawnArgs.impactRotation * Quaternion.Euler(Vector3.right * 90f);
                    break;
            }


            if (autoDestroy) {
                var destroyDuration = destroyDelayDuration;
                if (destroyDelayDuration <= 0)
                    if (gameObject.TryGetComponent<ParticleSystem>(out var particleSystem))
                        destroyDuration = particleSystem.main.duration;

                // GameManager.Spawner.Despawn(spawned, destroyDuration);
            }
        }

        public class SpawnEffectArgs : EffectArgs {
            public Vector3 impactPosition;
            public Quaternion impactRotation;

            protected override void Reset() {
                base.Reset();
                impactPosition = default;
                impactRotation = default;
            }
        }
    }

    [Serializable]
    public class CameraEffect : Effect {
        [SerializeField] private float shakeTime;
        [SerializeField] private float shakeAmplitude;

        public override void DoEffect<T>(T extraArgs) {
            // GameManager.CameraController.Shake(shakeTime, shakeAmplitude);
        }
    }

    [Serializable]
    public class TweenEffect : Effect {
        // [SerializeField]
        // private Vector3 vector3;

        public static Dictionary<GameObject, Tweener> activeTweeners = new();
        [SerializeField] private float duration = 1;

        [SerializeField] private float strength = 1;

        public override void DoEffect<T>(T extraArgs) {
            switch (spawnMode) {
                case Mode.Target:
                    if (activeTweeners.TryGetValue(extraArgs.target, out var tweener)) {
                        tweener.Rewind(false);
                        tweener.Kill();
                        activeTweeners.Remove(extraArgs.target);
                    }

                    activeTweeners[extraArgs.target] =
                        extraArgs.target.transform.DOPunchScale(Vector3.one * strength, duration);
                    break;

                case Mode.Instigator:
                    if (activeTweeners.TryGetValue(extraArgs.instigator, out tweener)) {
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

    [Serializable]
    public class AudioEffect : Effect {
        [SerializeField] private AudioClip audioClip;

        [SerializeField] [MinMaxSlider(0f, 2f)]
        private Vector2 pitch = Vector2.one;

        [SerializeField] [MinMaxSlider(0f, 2f)]
        private Vector2 volume = Vector2.one;

        public override void DoEffect<T>(T extraArgs) {
            // GameManager.Audio.PlayAudioInWorld(audioClip, GetLocation(extraArgs).point, pitch, volume);
        }
    }

    [Serializable]
    public class BuildBlockEffect : Effect {
        [SerializeField] private bool remove;

        public override void DoEffect<T>(T extraArgs) {
            // var spawnArgs = extraArgs as SpawnEffect.SpawnEffectArgs;
            //
            // var gridHolder = Object.FindObjectOfType<BlockGridHolder>();
            // var impactNormal = (spawnArgs.impactRotation * Vector3.forward).normalized;
            //
            // if (remove)
            //     gridHolder.RemoveBlock(spawnArgs.impactPosition + impactNormal);
            // else
            //     gridHolder.PlaceBlock(spawnArgs.impactPosition + impactNormal, 0);
            //
            // gridHolder.Run();
        }
    }

    [Serializable]
    public class PhysicsEffect : Effect {
        [SerializeField] private float force;
        [SerializeField] private float radius;
        [SerializeField] private ForceMode mode;

        public override void DoEffect<T>(T extraArgs) {
            var spawnArgs = extraArgs as SpawnEffect.SpawnEffectArgs;
            var impactNormal = (spawnArgs.impactRotation * Vector3.forward).normalized;
            var impactPosition = spawnArgs.impactPosition;

            var colliders = Physics.OverlapSphere(impactPosition, radius);
            if (colliders.Any())
                foreach (var collider in colliders) {
                    if (collider.attachedRigidbody == null) continue;
                    var forceVector = collider.transform.position - impactPosition;
                    forceVector = forceVector.normalized;
                    forceVector *= force;
                    collider.attachedRigidbody.AddForceAtPosition(forceVector, impactPosition, mode);
                }
        }
    }
}