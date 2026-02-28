using UnityEngine;

public static class MaterialHelper
{
    public const string UrpUnlitShader = "Universal Render Pipeline/Unlit";

    public static Material CreateUnlitTransparent(Color color)
    {
        Material mat = new Material(Shader.Find(UrpUnlitShader));
        mat.color = color;
        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_Blend", 0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.renderQueue = 3000;
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        return mat;
    }
}
