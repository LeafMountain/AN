using System;
using DG.Tweening;
using EventManager;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : NetworkBehaviour 
{
    [Serializable]
    public struct MapPoint
    {
        public enum PointType
        {
            None,
            Muzzle
        }

        public PointType pointType;
        

        public bool manual;
        [ShowIf(nameof(manual), false)]
        public Transform transform;
        [ShowIf(nameof(manual), true)]
        public Vector3 point;
        [ShowIf(nameof(manual), true)]
        public Vector3 rotation;
    }
    
    public UIActor ui;
    public MapPoint[] actorMapping;

    public MapPoint GetPoint(MapPoint.PointType pointType)
    {
        return Array.Find(actorMapping, point => point.pointType == pointType);
    }

    public (Vector3 position, Quaternion rotation) GetPointAndRotation(MapPoint.PointType pointType)
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
        if (ui)
        {
            Vector3 position = GetCenterOfMass() + Vector3.up * 1.5f;
            Quaternion rotation = transform.rotation;
            ui = GameManager.Spawn(ui, position, rotation, transform);
            ui.Init(this);
        }
        
        Events.AddListener(Flag.DamageRecieved, this, OnDamaged);
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
