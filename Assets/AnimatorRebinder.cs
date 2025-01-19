using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class AnimatorRebinder : MonoBehaviour
{
    public Animator animator;
    public Transform from;
    public Transform to;

    private void OnEnable()
    {
        Rebind();
    }

    [Button]
    public void Rebind()
    {
        SkinnedMeshRenderer[] oldRenderers = from.GetComponentsInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] newRenderers = to.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        foreach (var oldRenderer in oldRenderers)
        {
            var oldName = oldRenderer.name.Remove(oldRenderer.name.Length - 3, 3);
            var newRenderer = newRenderers.FirstOrDefault(x => x.name.Remove(x.name.Length - 3, 3) == oldName);
            if(newRenderer == null) continue;
            newRenderer.bones = oldRenderer.bones;
            newRenderer.rootBone = oldRenderer.rootBone;
        }
        
        animator.Rebind();     
    }
}
