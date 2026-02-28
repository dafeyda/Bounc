using UnityEngine;

// Added to the ball by BallSpawner.
// Draws a coloured line (BTL) from the ball along its current velocity direction.
// The line stops at the first solid surface it hits.
// Exception: if that surface is the player's paddle, a reflected segment is
// also drawn showing where the ball would go after the bounce.
[RequireComponent(typeof(Rigidbody))]
public class BallTrajectory : MonoBehaviour
{
    [SerializeField] Color lineColor = Color.green;

    private Rigidbody    _rb;
    private LineRenderer _line;
    private Collider     _playerCollider;

    // Pre-allocated; avoids per-frame heap allocations.
    // Slot 0 = ball position, 1 = first hit, 2 = reflected hit (optional).
    private readonly Vector3[] _pts = new Vector3[3];

    public static void Setup(GameObject ball) => ball.AddComponent<BallTrajectory>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _line = gameObject.AddComponent<LineRenderer>();
        _line.material             = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        _line.startColor           = Color.white;   // vertex colour × mat colour = lineColor
        _line.endColor             = Color.white;
        _line.startWidth           = 0.05f;
        _line.endWidth             = 0.05f;
        _line.useWorldSpace        = true;
        _line.receiveShadows       = false;
        _line.shadowCastingMode    = UnityEngine.Rendering.ShadowCastingMode.Off;
        _line.generateLightingData = false;
        _line.positionCount        = 0;
    }

    private void Start()
    {
        // Player is tagged "Player" in the scene.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            _playerCollider = player.GetComponent<Collider>();
    }

    // ── Per-frame update ──────────────────────────────────────────────────────

    private void Update()
    {
        _line.material.color = lineColor;   // picks up live Inspector edits

        Vector3 vel = _rb.linearVelocity;
        if (vel.sqrMagnitude < 0.01f)
        {
            _line.positionCount = 0;
            return;
        }

        Vector3 dir    = vel.normalized;
        float   radius = transform.lossyScale.x * 0.5f;      // ball radius in world space
        Vector3 origin = transform.position + dir * (radius + 0.02f); // start just outside the ball
        const float maxDist = 200f;

        _pts[0] = transform.position;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDist))
        {
            _pts[1] = hit.point;

            bool hitPlayer = _playerCollider != null && hit.collider == _playerCollider;
            if (hitPlayer)
            {
                // Compute reflection and cast a second segment.
                Vector3 reflDir    = Vector3.Reflect(dir, hit.normal);
                Vector3 reflOrigin = hit.point + reflDir * 0.02f;

                _pts[2] = Physics.Raycast(reflOrigin, reflDir, out RaycastHit hit2, maxDist)
                    ? hit2.point
                    : reflOrigin + reflDir * maxDist;

                _line.positionCount = 3;
            }
            else
            {
                _line.positionCount = 2;
            }
        }
        else
        {
            // Shouldn't happen inside the bounded arena, but handle gracefully.
            _pts[1] = origin + dir * maxDist;
            _line.positionCount = 2;
        }

        _line.SetPositions(_pts);
    }
}
