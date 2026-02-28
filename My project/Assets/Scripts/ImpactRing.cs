using UnityEngine;

public class ImpactRing : MonoBehaviour
{
    private LineRenderer _line;
    private Material _mat;
    private float _elapsed;

    private const float Duration = 0.35f;
    private const float MaxRadius = 1.2f;
    private const int Segments = 48;
    private static readonly Color RingColor = new Color(1f, 0.7f, 0.2f);

    public static void Spawn(ContactPoint contact)
    {
        GameObject go = new GameObject("ImpactRing");
        // Offset slightly off the surface along the normal to avoid z-fighting
        go.transform.position = contact.point + contact.normal * 0.02f;
        // Align local Y with the surface normal so the circle lies parallel to the surface
        go.transform.rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);

        go.AddComponent<ImpactRing>().Init();
    }

    private void Init()
    {
        _mat = MaterialHelper.CreateUnlitTransparent(RingColor);

        _line = gameObject.AddComponent<LineRenderer>();
        _line.loop = true;
        _line.positionCount = Segments;
        _line.startWidth = 0.06f;
        _line.endWidth = 0.06f;
        _line.useWorldSpace = false;
        _line.material = _mat;

        SetRadius(0f, 1f);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float t = _elapsed / Duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        float radius = Mathf.Lerp(0f, MaxRadius, t);
        float alpha = Mathf.Lerp(1f, 0f, t);
        SetRadius(radius, alpha);
    }

    private void SetRadius(float radius, float alpha)
    {
        for (int i = 0; i < Segments; i++)
        {
            float angle = (float)i / Segments * Mathf.PI * 2f;
            // Draw in local XZ plane so the ring lies flat relative to the surface normal
            _line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }

        Color c = new Color(RingColor.r, RingColor.g, RingColor.b, alpha);
        _line.startColor = c;
        _line.endColor = c;
    }
}
