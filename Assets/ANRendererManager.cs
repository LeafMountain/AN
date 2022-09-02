using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ANRendererManager : MonoBehaviour
{
    Dictionary<(Mesh mesh, Material mat, int subMesh), List<Matrix4x4>> instances = new();

    private void Start()
    {
        var renderers = FindObjectsOfType<RenderingHelper>();

        for (var i = 0; i < renderers.Length; i++)
        {
            for (int subMesh = 0; subMesh < renderers[i].mesh.subMeshCount; subMesh++)
            {
                var key = (renderers[i].mesh, renderers[i].materials[subMesh], j: subMesh);
                if (instances.ContainsKey(key) == false) instances[key] = new List<Matrix4x4>();
                instances[key].Add(renderers[i].transform.localToWorldMatrix);
            }
        }
    }

    void Update()
    {
        if(instances.Count == 0) Start();
        
        foreach (var keyValuePair in instances)
        {
            Graphics.DrawMeshInstanced(keyValuePair.Key.mesh, keyValuePair.Key.subMesh, keyValuePair.Key.mat, keyValuePair.Value);
        }
    }
}