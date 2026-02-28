using UnityEngine;

// Camera 4: keeps both the player and ball in frame at all times.
// Floats above their midpoint at a height that scales with their separation.
public class MidpointCamera : MonoBehaviour
{
    [SerializeField] private float minHeight    = 8f;
    [SerializeField] private float spreadFactor = 0.7f;
    [SerializeField] private float smoothTime   = 0.1f;

    private Transform _player;
    private Transform _ball;
    private Vector3   _velocity;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _player = player.transform;
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        if (_ball == null) _ball = SceneHelper.FindBall();

        Vector3 mid    = _ball != null ? (_player.position + _ball.position) * 0.5f : _player.position;
        float   spread = _ball != null ? Vector3.Distance(_player.position, _ball.position) : 0f;

        float height = Mathf.Max(minHeight, spread * spreadFactor);

        // Hover above the midpoint with a slight south offset for a clear 3/4 view
        Vector3 desired = mid + Vector3.up * height + Vector3.back * (height * 0.35f);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        transform.LookAt(mid);
    }
}
