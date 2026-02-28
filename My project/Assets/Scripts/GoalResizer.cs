using UnityEngine;

// Attached to a Goal_North or Goal_South parent object.
// Grow() expands the goal opening when the surrounding arena wall is hit.
// ResetSize() shrinks it back to its starting size on a goal score.
public class GoalResizer : MonoBehaviour
{
    [SerializeField] private float widthGrowth  = 0.8f;
    [SerializeField] private float heightGrowth = 0.4f;

    private const float WallCenterY = ArenaConstants.Height / 2f;

    private float _w, _h;
    private float _initialW, _initialH;
    private float _outwardDir;
    private float _wallZ;

    private Transform _alcoveLeft, _alcoveRight, _alcoveTop, _alcoveBottom, _back;
    private Transform _wallLeft, _wallRight, _wallBottom, _wallTop;

    private void Start()
    {
        bool isNorth = name.Contains("North");
        _outwardDir = isNorth ? 1f : -1f;

        // Find alcove children
        _alcoveLeft   = transform.Find("Left");
        _alcoveRight  = transform.Find("Right");
        _alcoveTop    = transform.Find("Top");
        _alcoveBottom = transform.Find("Bottom");
        _back         = transform.Find("Back");

        // Derive current dimensions from Back wall scale
        if (_back != null)
        {
            _w = _back.localScale.x - 2f * ArenaConstants.GoalPieceThick;
            _h = _back.localScale.y - 2f * ArenaConstants.GoalPieceThick;
        }
        _initialW = _w;
        _initialH = _h;

        // Find surrounding wall pieces (siblings under Arena)
        string side = isNorth ? "North" : "South";
        Transform arena = transform.parent;
        _wallLeft   = arena.Find($"Wall_{side}_Left");
        _wallRight  = arena.Find($"Wall_{side}_Right");
        _wallBottom = arena.Find($"Wall_{side}_Bottom");
        _wallTop    = arena.Find($"Wall_{side}_Top");

        _wallZ = _wallLeft != null ? _wallLeft.position.z : (isNorth ? 20f : -20f);

        if (_back == null)
            Debug.LogWarning($"[GoalResizer] Could not find 'Back' child on '{name}'");
        if (_wallLeft == null || _wallRight == null || _wallBottom == null || _wallTop == null)
            Debug.LogWarning($"[GoalResizer] One or more Wall_{side}_* siblings not found under Arena");

        // Attach WallHitGrow to all four goal wall sections (not east/west arena walls).
        AttachWallHit(_wallLeft);
        AttachWallHit(_wallRight);
        AttachWallHit(_wallBottom);
        AttachWallHit(_wallTop);
    }

    private void AttachWallHit(Transform wall)
    {
        if (wall == null) return;
        if (wall.GetComponent<WallHitGrow>() != null) return;
        wall.gameObject.AddComponent<WallHitGrow>().goalResizer = this;
    }

    public void Grow()
    {
        _w = Mathf.Min(_w + widthGrowth,  ArenaConstants.Width  - 2f);
        _h = Mathf.Min(_h + heightGrowth, ArenaConstants.Height - 2f);
        UpdateGeometry();
    }

    public void ResetSize()
    {
        _w = _initialW;
        _h = _initialH;
        UpdateGeometry();
    }

    private void UpdateGeometry()
    {
        float W    = _w;
        float H    = _h;
        float T    = ArenaConstants.GoalPieceThick;
        float D    = ArenaConstants.GoalAlcoveDepth;
        float outD = _outwardDir * D;

        // ── Alcove pieces (local space) ──────────────────────────────────────────
        if (_alcoveLeft)
        {
            _alcoveLeft.localPosition = new Vector3(-W / 2f, 0f, outD / 2f);
            _alcoveLeft.localScale    = new Vector3(T, H, D);
        }
        if (_alcoveRight)
        {
            _alcoveRight.localPosition = new Vector3(W / 2f, 0f, outD / 2f);
            _alcoveRight.localScale    = new Vector3(T, H, D);
        }
        if (_alcoveTop)
        {
            _alcoveTop.localPosition = new Vector3(0f, H / 2f, outD / 2f);
            _alcoveTop.localScale    = new Vector3(W + 2f * T, T, D);
        }
        if (_alcoveBottom)
        {
            _alcoveBottom.localPosition = new Vector3(0f, -H / 2f, outD / 2f);
            _alcoveBottom.localScale    = new Vector3(W + 2f * T, T, D);
        }
        if (_back)
        {
            _back.localScale = new Vector3(W + 2f * T, H + 2f * T, T);
        }

        // ── Surrounding wall pieces (world space) ────────────────────────────────
        float goalMinY = WallCenterY - H / 2f;
        float goalMaxY = WallCenterY + H / 2f;
        float sideW    = (ArenaConstants.Width - W) / 2f;
        float topH     = ArenaConstants.Height - goalMaxY;

        if (_wallLeft)
        {
            _wallLeft.position   = new Vector3(-(W / 2f + sideW / 2f), WallCenterY, _wallZ);
            _wallLeft.localScale = new Vector3(sideW, ArenaConstants.Height, ArenaConstants.WallThick);
        }
        if (_wallRight)
        {
            _wallRight.position   = new Vector3(W / 2f + sideW / 2f, WallCenterY, _wallZ);
            _wallRight.localScale = new Vector3(sideW, ArenaConstants.Height, ArenaConstants.WallThick);
        }
        if (_wallBottom)
        {
            _wallBottom.position   = new Vector3(0f, goalMinY / 2f, _wallZ);
            _wallBottom.localScale = new Vector3(W, goalMinY, ArenaConstants.WallThick);
        }
        if (_wallTop)
        {
            _wallTop.position   = new Vector3(0f, goalMaxY + topH / 2f, _wallZ);
            _wallTop.localScale = new Vector3(W, topH, ArenaConstants.WallThick);
        }
    }
}
