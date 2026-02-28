using UnityEngine;

// Draws a small 3D arrow centred on the player, pointing in the direction
// the player will move when W is pressed.
// Camera 1  → world +Z (north).
// All other cameras → camera-forward flattened to horizontal.
// Purely visual — no colliders.
public class DirectionArrow : MonoBehaviour
{
    private Transform _root;

    private void Start()
    {
        _root = BuildArrow();
    }

    private Transform BuildArrow()
    {
        Transform root = new GameObject("DirectionArrow").transform;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 0.85f, 0f); // golden yellow

        // Shaft — extends from origin along local +Z
        MakePart(root, mat, "Shaft",
            localPos:   new Vector3(0f,      0f, 0.25f),
            localRot:   Quaternion.identity,
            localScale: new Vector3(0.07f, 0.07f, 0.5f));

        // Left wing — angles backward-left from the tip
        MakePart(root, mat, "WingL",
            localPos:   new Vector3(-0.106f, 0f, 0.394f),
            localRot:   Quaternion.Euler(0f, -135f, 0f),
            localScale: new Vector3(0.07f, 0.07f, 0.3f));

        // Right wing — angles backward-right from the tip
        MakePart(root, mat, "WingR",
            localPos:   new Vector3( 0.106f, 0f, 0.394f),
            localRot:   Quaternion.Euler(0f,  135f, 0f),
            localScale: new Vector3(0.07f, 0.07f, 0.3f));

        return root;
    }

    private void MakePart(Transform parent, Material mat, string partName,
        Vector3 localPos, Quaternion localRot, Vector3 localScale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = partName;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;
        go.transform.localScale    = localScale;

        Destroy(go.GetComponent<Collider>());

        go.GetComponent<MeshRenderer>().material = mat;
    }

    private void LateUpdate()
    {
        if (_root == null) return;

        // Sit at the player's centre — superimposed on the plane
        _root.position = transform.position;

        // Orient in world space only — independent of the player's tilt
        Vector3 dir = GetWMoveDirection();
        _root.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private Vector3 GetWMoveDirection()
    {
        bool isCamera1 = CameraManager.Instance == null
            || CameraManager.Instance.ActiveCameraIndex == 1;

        if (isCamera1)
            return Vector3.forward; // Camera 1: W = world north (+Z)

        Camera cam = CameraManager.Instance.ActiveCamera;
        if (cam == null) return Vector3.forward;

        Vector3 dir = cam.transform.forward;
        dir.y = 0f;
        return dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;
    }

    private void OnDestroy()
    {
        if (_root != null)
            Destroy(_root.gameObject);
    }
}
