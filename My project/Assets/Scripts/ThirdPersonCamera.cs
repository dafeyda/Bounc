using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Follow Target (Player)")]
    [SerializeField] private Transform followTarget;

    [Header("Look Target (Ball)")]
    [SerializeField] private Transform lookTarget;

    [Header("Position Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float heightOffset = 5f;
    [SerializeField] private float positionSmoothTime = 0.1f;

    [Header("Arena Bounds")]
    [SerializeField] private float boundX    = 19f;
    [SerializeField] private float boundYMin =  0.5f;
    [SerializeField] private float boundYMax = 19f;
    [SerializeField] private float boundZ    = 19f;

    private Vector3 _currentVelocity;

    private void Start()
    {
        FindTargets();
    }

    private void FindTargets()
    {
        if (followTarget == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Transform camTarget = player.transform.Find("CameraTarget");
                followTarget = camTarget != null ? camTarget : player.transform;
            }
        }

        if (lookTarget == null)
            lookTarget = SceneHelper.FindBall();
    }

    private void LateUpdate()
    {
        if (followTarget == null || lookTarget == null) FindTargets();
        if (followTarget == null) return;

        Vector3 playerPos = followTarget.position;

        Vector3 lookAt;

        if (lookTarget != null)
        {
            Vector3 playerToBall   = (lookTarget.position - playerPos).normalized;
            Vector3 desiredPosition = playerPos - playerToBall * distance + Vector3.up * heightOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, positionSmoothTime);
            lookAt = lookTarget.position;
        }
        else
        {
            Vector3 desiredPosition = playerPos + Vector3.up * heightOffset - Vector3.forward * distance;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, positionSmoothTime);
            lookAt = playerPos;
        }

        // Clamp within arena so the camera never exits through a wall.
        // LookAt is called after clamping so the ball stays centred even when pinned.
        Vector3 p = transform.position;
        transform.position = new Vector3(
            Mathf.Clamp(p.x, -boundX,    boundX),
            Mathf.Clamp(p.y,  boundYMin, boundYMax),
            Mathf.Clamp(p.z, -boundZ,    boundZ));

        transform.LookAt(lookAt);
    }

}
