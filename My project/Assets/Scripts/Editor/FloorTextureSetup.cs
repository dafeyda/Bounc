using UnityEditor;
using UnityEngine;

public static class FloorTextureSetup
{
    [MenuItem("Tools/Setup Floor Texture")]
    public static void Setup()
    {
        SetupTexture("Assets/Textures/Checker.png");
        SetupMaterial("Assets/Materials/FloorGrid.mat", 20, 20);
    }

    [MenuItem("Tools/Setup Wall Texture")]
    public static void SetupWalls()
    {
        SetupTexture("Assets/Textures/WallSciFi.png");
        SetupMaterial("Assets/Materials/WallSciFi.mat", 10, 10);
    }

    private static void SetupTexture(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static void SetupMaterial(string path, float tilingX, float tilingY)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null)
        {
            mat.SetTextureScale("_BaseMap", new Vector2(tilingX, tilingY));
            mat.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();
            Debug.Log($"Texture setup complete for {path} - tiling {tilingX}x{tilingY}");
        }
    }
}
