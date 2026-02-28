using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GoalSetup
{
    private const float GoalWidth   = ArenaConstants.Width  / 3f;        // ~13.333
    private const float GoalHeight  = ArenaConstants.Height * 3f / 8f;  // 7.5

    [MenuItem("Tools/Create Goals")]
    public static void CreateGoals()
    {
        GameObject arena = GameObject.Find("Arena");
        if (arena == null) { Debug.LogError("Arena not found."); return; }

        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/WallSciFi.mat");

        // outwardDir: direction away from arena interior (into the wall)
        SetupWallWithGoal(arena, "North", wallZ:  20f, outwardDir:  1f, mat);
        SetupWallWithGoal(arena, "South", wallZ: -20f, outwardDir: -1f, mat);

        EditorSceneManager.MarkSceneDirty(arena.scene);
        Debug.Log("Goals created successfully.");
    }

    private static void SetupWallWithGoal(GameObject arena, string side, float wallZ, float outwardDir, Material mat)
    {
        float W  = GoalWidth;
        float H  = GoalHeight;
        float D  = ArenaConstants.GoalAlcoveDepth;
        float T  = ArenaConstants.GoalPieceThick;
        float AW = ArenaConstants.Width;
        float AH = ArenaConstants.Height;

        float innerFaceZ  = wallZ - outwardDir * (ArenaConstants.WallThick / 2f);
        float wallCenterY = AH / 2f;          // y = 10 (center of wall)
        float goalMinY    = wallCenterY - H / 2f;  // y = 7.5
        float goalMaxY    = wallCenterY + H / 2f;  // y = 12.5
        float sideW       = (AW - W) / 2f;    // width of left/right wall sections = 16.667

        // Reference texel density from the left/right pieces (tiling 10x10 on sideW x AH)
        float refDensityH = 10f / sideW;   // tiles per world unit, horizontal
        float refDensityV = 10f / AH;      // tiles per world unit, vertical

        // Unity's cube -Z face (north wall, visible from inside) has V running top→bottom,
        // opposite to the +Z face (south wall). We must account for this when computing offsets.
        bool vFlipped = (outwardDir > 0f);

        // ── Clean up any previous run ─────────────────────────────────────────
        DestroyIfExists(arena, $"Wall_{side}");
        DestroyIfExists(arena, $"Wall_{side}_Left");
        DestroyIfExists(arena, $"Wall_{side}_Right");
        DestroyIfExists(arena, $"Wall_{side}_Bottom");
        DestroyIfExists(arena, $"Wall_{side}_Top");
        DestroyIfExists(arena, $"Goal_{side}");

        // ── Rebuild the wall as 4 pieces around the goal opening ──────────────

        // Left section: full height, left of goal
        MakeWall(arena, $"Wall_{side}_Left",
            pos:    new Vector3(-(W / 2f + sideW / 2f), wallCenterY, wallZ),
            scale:  new Vector3(sideW, AH, ArenaConstants.WallThick), mat,
            tiling: new Vector2(10f, 10f),
            offset: Vector2.zero);

        // Right section: full height, right of goal
        MakeWall(arena, $"Wall_{side}_Right",
            pos:    new Vector3(W / 2f + sideW / 2f, wallCenterY, wallZ),
            scale:  new Vector3(sideW, AH, ArenaConstants.WallThick), mat,
            tiling: new Vector2(10f, 10f),
            offset: Vector2.zero);

        // Bottom section: tiling scaled to match left/right texel density
        float topH          = AH - goalMaxY;
        float centerTilingX = W        * refDensityH;
        float bottomTilingY = goalMinY * refDensityV;
        float topTilingY    = topH     * refDensityV;

        // Compute the texture V phase at each join point as seen by the left/right pieces.
        // When vFlipped, the left piece's V=0 is at the top (high Y), so phase runs in reverse.
        float phaseAtGoalMinY = vFlipped
            ? ((AH - goalMinY) * refDensityV) % 1f
            : (goalMinY        * refDensityV) % 1f;
        float phaseAtGoalMaxY = vFlipped
            ? ((AH - goalMaxY) * refDensityV) % 1f
            : (goalMaxY        * refDensityV) % 1f;

        // Bottom piece: its top edge must match phaseAtGoalMinY.
        //   Not flipped → top edge has V=1 → offset = (phase - tilingY) mod 1
        //   Flipped     → top edge has V=0 → offset = phase
        float bottomVOffset = vFlipped
            ? phaseAtGoalMinY
            : ((phaseAtGoalMinY - bottomTilingY % 1f + 1f) % 1f);

        // Top piece: its bottom edge must match phaseAtGoalMaxY.
        //   Not flipped → bottom edge has V=0 → offset = phase
        //   Flipped     → bottom edge has V=1 → offset = (phase - tilingY) mod 1
        float topVOffset = vFlipped
            ? ((phaseAtGoalMaxY - topTilingY % 1f + 1f) % 1f)
            : phaseAtGoalMaxY;

        MakeWall(arena, $"Wall_{side}_Bottom",
            pos:    new Vector3(0f, goalMinY / 2f, wallZ),
            scale:  new Vector3(W, goalMinY, ArenaConstants.WallThick), mat,
            tiling: new Vector2(centerTilingX, bottomTilingY),
            offset: new Vector2(0f, bottomVOffset));

        MakeWall(arena, $"Wall_{side}_Top",
            pos:    new Vector3(0f, goalMaxY + topH / 2f, wallZ),
            scale:  new Vector3(W, topH, ArenaConstants.WallThick), mat,
            tiling: new Vector2(centerTilingX, topTilingY),
            offset: new Vector2(0f, topVOffset));

        // ── Build the recessed goal alcove ────────────────────────────────────
        GameObject goalParent = new GameObject($"Goal_{side}");
        goalParent.transform.SetParent(arena.transform, worldPositionStays: false);
        goalParent.transform.position = new Vector3(0f, wallCenterY, innerFaceZ);

        float outD = outwardDir * D;

        // Side walls (left/right), top, bottom — span the alcove depth
        MakeAlcove(goalParent, "Left",   new Vector3(-W / 2f,  0f,      outD / 2f), new Vector3(T, H, D), mat);
        MakeAlcove(goalParent, "Right",  new Vector3( W / 2f,  0f,      outD / 2f), new Vector3(T, H, D), mat);
        MakeAlcove(goalParent, "Top",    new Vector3( 0f,       H / 2f,  outD / 2f), new Vector3(W + 2 * T, T, D), mat);
        MakeAlcove(goalParent, "Bottom", new Vector3( 0f,      -H / 2f,  outD / 2f), new Vector3(W + 2 * T, T, D), mat);

        // Back wall — at the far end of the alcove
        MakeAlcove(goalParent, "Back", new Vector3(0f, 0f, outD), new Vector3(W + 2 * T, H + 2 * T, T), mat);
    }

    // Wall piece: no shadow casting so directional light reaches interior
    private static void MakeWall(GameObject arena, string pieceName, Vector3 pos, Vector3 scale, Material mat, Vector2 tiling, Vector2 offset)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = pieceName;
        go.transform.SetParent(arena.transform, worldPositionStays: false);
        go.transform.position   = pos;
        go.transform.localScale = scale;

        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        // Create a per-instance material so tiling/offset don't affect the shared asset
        var instance = new Material(mat);
        instance.name = mat.name + " (Instance)";
        instance.SetTextureScale("_BaseMap", tiling);
        instance.SetTextureOffset("_BaseMap", offset);
        instance.SetTextureScale("_MainTex", tiling);
        instance.SetTextureOffset("_MainTex", offset);
        mr.sharedMaterial = instance;

        EditorUtility.SetDirty(instance);
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // Alcove piece: inside the wall, can cast shadows
    private static void MakeAlcove(GameObject parent, string pieceName, Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = pieceName;
        go.transform.SetParent(parent.transform, worldPositionStays: false);
        go.transform.localPosition = localPos;
        go.transform.localScale    = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private static void DestroyIfExists(GameObject arena, string name)
    {
        Transform t = arena.transform.Find(name);
        if (t != null) Object.DestroyImmediate(t.gameObject);
    }
}
