using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AIOpponent : MonoBehaviour
{
    [SerializeField] float moveSpeed   = 5f;
    [SerializeField] float rotateSpeed = 120f; // degrees per second

    private static readonly Vector3 HomePosition =
        new Vector3(0f, 1f, ArenaConstants.HalfDepth - 5f);   // (0, 1, 15)

    private static readonly Vector3 GoalSouthCenter =
        new Vector3(0f, 1f, -ArenaConstants.HalfDepth);        // (0, 1, -20)

    private CharacterController _controller;
    private Transform           _ball;
    private Rigidbody           _ballRb;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_ball == null)
        {
            _ball = SceneHelper.FindBall();
            if (_ball != null)
                _ballRb = _ball.GetComponent<Rigidbody>();
            if (_ball == null) return;
        }

        Move();
        RotateToAim();
    }

    // ── Movement ─────────────────────────────────────────────────────────────

    private void Move()
    {
        Vector3 target;

        if (_ball.position.z > 0f)
        {
            // Ball in AI's half: slide in XY only, keeping Z fixed at home.
            // The ball naturally travels to Z=15 and hits the paddle; the AI
            // never chases along Z, so it cannot inject momentum into the ball.
            target = new Vector3(
                Mathf.Clamp(_ball.position.x,
                    -ArenaConstants.HalfWidth + 1f, ArenaConstants.HalfWidth - 1f),
                Mathf.Clamp(_ball.position.y, 1f, ArenaConstants.Height - 1f),
                HomePosition.z
            );
        }
        else
        {
            target = HomePosition;
        }

        Vector3 dir = target - transform.position;
        if (dir.sqrMagnitude > 0.01f)
            _controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

    // ── Rotation ─────────────────────────────────────────────────────────────

    private void RotateToAim()
    {
        if (_ballRb == null) return;

        // Incoming ball direction (fall back to AI→ball direction when slow).
        Vector3 incoming = _ballRb.linearVelocity.sqrMagnitude > 0.1f
            ? _ballRb.linearVelocity.normalized
            : (_ball.position - transform.position).normalized;

        // Direction we want the ball to travel after bouncing off the paddle.
        Vector3 desired = (GoalSouthCenter - _ball.position).normalized;

        // Physics: the surface normal that reflects 'incoming' toward 'desired'
        // is the normalised half-vector of (-incoming) and desired.
        Vector3 n = -incoming + desired;
        if (n.sqrMagnitude < 0.01f)
            n = _ball.position - transform.position;   // fallback: face the ball
        n.Normalize();

        // Build an orthonormal basis with local Y = n and no banking
        // (local X stays as horizontal as possible).
        Vector3 right = Vector3.ProjectOnPlane(Vector3.right, n);
        if (right.sqrMagnitude < 0.001f)
            right = Vector3.ProjectOnPlane(Vector3.forward, n);
        right.Normalize();
        Vector3 fwd = Vector3.Cross(right, n);   // local Z

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(fwd, n),
            rotateSpeed * Time.deltaTime);
    }
}
