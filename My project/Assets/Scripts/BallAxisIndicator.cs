using UnityEngine;

// Draws three axis lines through the ball extending to the arena walls.
public class BallAxisIndicator : MonoBehaviour
{

    private LineRenderer _xLine, _yLine, _zLine;

    public static void Setup(GameObject ball)
    {
        ball.AddComponent<BallAxisIndicator>();
    }

    private void Awake()
    {
        _xLine = CreateLine(new Color(1f,   0.3f, 0.3f, 0.45f));
        _yLine = CreateLine(new Color(1f,   0.85f, 0.1f, 0.45f)); // yellow — distinct from BTL green
        _zLine = CreateLine(new Color(0.3f, 0.6f, 1f,   0.45f));
    }

    private LineRenderer CreateLine(Color color)
    {
        GameObject go = new GameObject("AxisLine");
        go.transform.SetParent(transform, worldPositionStays: false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount     = 2;
        lr.startWidth        = 0.08f;
        lr.endWidth          = 0.08f;
        lr.useWorldSpace     = true;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows    = false;

        Material mat = MaterialHelper.CreateUnlitTransparent(color);

        lr.material   = mat;
        lr.startColor = color;
        lr.endColor   = color;
        return lr;
    }

    private void LateUpdate()
    {
        Vector3 p = transform.position;

        _xLine.SetPosition(0, new Vector3(-ArenaConstants.HalfWidth, p.y, p.z));
        _xLine.SetPosition(1, new Vector3( ArenaConstants.HalfWidth, p.y, p.z));
        _yLine.SetPosition(0, new Vector3(p.x, ArenaConstants.Floor,  p.z));
        _yLine.SetPosition(1, new Vector3(p.x, ArenaConstants.Height, p.z));
        _zLine.SetPosition(0, new Vector3(p.x, p.y, -ArenaConstants.HalfDepth));
        _zLine.SetPosition(1, new Vector3(p.x, p.y,  ArenaConstants.HalfDepth));
    }
}
