using UnityEngine;

[ExecuteInEditMode]
public class RenderingHelper : MonoBehaviour
{
    public Mesh mesh;
    public Material[] materials;

    public GameObject go;

    private void OnValidate()
    {
        var test = go.GetComponent<MeshFilter>();
        mesh = test.sharedMesh;
        var test2 = go.GetComponent<Renderer>();
        materials = test2.sharedMaterials;
    }

    // private void Update()
    // {
    //     var test = go.GetComponent<MeshFilter>();
    //     mesh = test.sharedMesh;
    //     var test2 = go.GetComponent<Renderer>();
    //     materials = test2.sharedMaterials;
    //     
    //     for (int i = 0; i < mesh.subMeshCount; i++)
    //     {
    //         // Graphics.RenderMesh(new RenderParams(materials[i]), mesh, i, transform.localToWorldMatrix);
    //         Graphics.DrawMeshInstanced(mesh, i, materials[i],  new []{ transform.localToWorldMatrix });
    //     }
    // }
}
