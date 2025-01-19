using System;
using System.Linq;
using DG.Tweening;
using EventManager;
using InventorySystem;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [DisallowMultipleComponent, RequireComponent(typeof(NetworkIdentity))]
    public class Actor : NetworkBehaviour {
        public ActorHandle handle = ActorHandle.Create();
        
        [Serializable]
        public struct MapPoint
        {
            // public enum PointType
            // {
                // None,
                // Muzzle
            // }

            public string pointType;
        

            public bool manual;
            [ShowIf(nameof(manual), false)]
            public Transform transform;
            [ShowIf(nameof(manual), true)]
            public Vector3 point;
            [ShowIf(nameof(manual), true)]
            public Vector3 rotation;
        }
    
        public MapPoint[] actorMapping;

        public MapPoint GetPoint(string pointType)
        {
            return Array.Find(actorMapping, point => point.pointType == pointType);
        }

        public (Vector3 position, Quaternion rotation) GetPointAndRotation(string pointType)
        {
            var value = GetPoint(pointType);
            if (value.manual == false)
            {
                if (value.transform == null)
                {
                    return (transform.position, transform.rotation);
                }
                return (value.transform.position, value.transform.rotation);
            }
            return (value.point, Quaternion.Euler(value.rotation));
        }

        protected virtual void Start()
        {
            Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
            var actorComponents = GetComponents<IActorComponent>();
            if (actorComponents.Any())
            {
                foreach (var actorComponent in actorComponents)
                {
                    actorComponent.OnParentInitialized();
                }
            }
        }

        protected virtual void Update()
        {
        }

        protected virtual void Reset()
        {
         
        }

        public virtual Vector3 GetCenterOfMass()
        {
            Vector3 centerOfMass = GetComponent<Collider>().bounds.center;
            return centerOfMass;
        }

        private Tweener tweener;
    
        protected virtual void OnDamaged(object origin, EventArgs eventargs)
        {

        }

        private void OnDrawGizmosSelected()
        {
            if (actorMapping != null)
            {
                foreach (var mapPoint in actorMapping)
                {
                    var (position, rotation) = GetPointAndRotation(mapPoint.pointType);
                
                    Gizmos.DrawSphere(position, .1f);
                    Gizmos.DrawRay(position, rotation * Vector3.forward);
                }
            }
        }
    }
}
