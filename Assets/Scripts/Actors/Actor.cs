using Mirror;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : NetworkBehaviour 
{
    public UIActor ui;

    protected virtual void Start()
    {
        if (ui)
        {
            ui = Instantiate(ui, transform.position + Vector3.up * 1.5f, transform.rotation, transform);
            ui.Init(this);
        }
    }
}
