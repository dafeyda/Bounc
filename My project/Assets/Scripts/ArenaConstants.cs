// Single source of truth for arena and goal dimensions.
// Referenced by BallAxisIndicator, GoalResizer, and GoalSetup.
public static class ArenaConstants
{
    // ── Arena extents ────────────────────────────────────────────────────────
    public const float Width     = 40f;
    public const float HalfWidth = Width / 2f;   // 20  — X extent
    public const float HalfDepth = 20f;           // Z extent (north/south)
    public const float Height    = 20f;           // Y extent (floor to ceiling)
    public const float Floor     = 0f;
    public const float WallThick = 1f;

    // ── Goal alcove ──────────────────────────────────────────────────────────
    public const float GoalAlcoveDepth = 1.5f;   // how far the alcove recesses into the wall
    public const float GoalPieceThick  = 0.2f;   // thickness of alcove side/top/bottom walls
}
