using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : NetworkBehaviour 
{
    public UIActor ui;

    protected virtual void Start()
    {
        if (ui)
        {
            ui = Instantiate(ui, GetCenterOfMass() + Vector3.up * 1.5f, transform.rotation, transform);
            ui.Init(this);
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
    
}
