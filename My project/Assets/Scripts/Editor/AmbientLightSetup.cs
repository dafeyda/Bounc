using UnityEngine;
using UnityEditor;

public static class AmbientLightSetup
{
    [MenuItem("Tools/Setup Ambient Light")]
    public static void SetupAmbient()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.25f, 0.25f, 0.3f); // cool dark fill
        RenderSettings.ambientIntensity = 1f;
        Debug.Log("Ambient light configured.");
    }
}
