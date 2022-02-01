using UnityEngine;

public class Loot : Actor
{
    [ColorUsage(true, true)]
    public Color color;
    public string colorPropertyName = "_BaseColor";
    public string emissiveColorPropertyName = "_EmissiveColor";

    protected override void Start()
    {
        base.Start();
        SetColor(color);
    }

    [ContextMenu("Set Color")]
    private void TestSetColor()
    {
        SetColor(color);
    }
    
    private void SetColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        var matPropBlock = new MaterialPropertyBlock();
        matPropBlock.SetColor(colorPropertyName, color);
        // matPropBlock.SetColor(colorPropertyName, color);
        renderer.SetPropertyBlock(matPropBlock);
        
        // renderer.sharedMaterial.SetColor(colorPropertyName, color);
        // renderer.sharedMaterial.SetColor(emissiveColorPropertyName, color);
    }
}
