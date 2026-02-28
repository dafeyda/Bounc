using UnityEngine;

// Camera 3: sits along the player's right axis (perpendicular to the surface normal),
// giving a profile view of the player's tilt and the ball's position relative to the plane.
public class NormalFollowCamera : MonoBehaviour
{
    [SerializeField] private float distance   = 7f;
    [SerializeField] private float smoothTime = 0.08f;

    private Transform _player;
    private Vector3   _velocity;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _player = player.transform;
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        // Sit 90° around the normal — along the player's right axis — for a profile view
        Vector3 desired = _player.position + _player.right * distance;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        transform.LookAt(_player.position);
    }
}
