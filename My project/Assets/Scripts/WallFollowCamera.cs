using UnityEngine;

// Wall-mounted camera that slides in X and Y to follow the player.
// Z is fixed at the wall. Facing direction is derived from which wall it sits on:
//   south wall (Z < 0) → faces north (Y=0°), north wall (Z > 0) → faces south (Y=180°).
public class WallFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float minCameraY   =  1f;   // floor boundary
    [SerializeField] private float maxCameraY   = 18f;   // ceiling boundary
    [SerializeField] private float maxCameraX   = 19f;   // left/right wall boundary

    private float  _fixedZ;
    private Camera _cam;

    private void Start()
    {
        _fixedZ = transform.position.z;
        _cam    = GetComponent<Camera>();

        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (!_cam.enabled) return;
        if (target == null) return;

        float camY = Mathf.Clamp(target.position.y + heightOffset, minCameraY, maxCameraY);
        float camX = Mathf.Clamp(target.position.x, -maxCameraX, maxCameraX);
        transform.position = new Vector3(camX, camY, _fixedZ);
        transform.LookAt(target.position);
    }
}
