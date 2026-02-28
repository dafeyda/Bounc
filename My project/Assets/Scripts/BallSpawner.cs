using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private float spawnHeight = 5f;
    [SerializeField] private float launchSpeed = 3f;
    [SerializeField] private float ballRadius = 0.5f;
    private void Start()
    {
        // Allow all collisions to bounce, even at low velocities
        Physics.bounceThreshold = 0f;
        SpawnBall();
    }

    private void SpawnBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = SceneHelper.BallName;
        ball.transform.position = new Vector3(0f, spawnHeight, 0f);
        ball.transform.localScale = Vector3.one * ballRadius * 2f;

        PhysicsMaterial bounceMat = PhysicsHelper.CreateBouncyMaterial();

        ball.GetComponent<SphereCollider>().material = bounceMat;

        // Ensure ball casts and receives shadows
        MeshRenderer ballRenderer = ball.GetComponent<MeshRenderer>();
        ballRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        ballRenderer.receiveShadows = true;

        // Apply bouncy material to all arena surfaces so bouncing is consistent
        ApplyBounceMaterial(bounceMat);

        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.5f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.linearVelocity = Random.onUnitSphere * launchSpeed;

        // Add bonk sound on collision
        BallSound.Setup(ball);

        // Add axis indicator lines for depth perception
        BallAxisIndicator.Setup(ball);

        // Add trajectory preview line
        BallTrajectory.Setup(ball);

        // Add trail renderer for depth perception
        TrailRenderer trail = ball.AddComponent<TrailRenderer>();
        trail.time = 1.5f;
        trail.startWidth = ballRadius * 1.5f;
        trail.endWidth = 0f;
        trail.material = MaterialHelper.CreateUnlitTransparent(new Color(0.3f, 0.6f, 1f, 0.6f));
        trail.startColor = new Color(0.3f, 0.6f, 1f, 0.6f);
        trail.endColor = new Color(0.3f, 0.6f, 1f, 0f);
        trail.minVertexDistance = 0.1f;
        trail.generateLightingData = false;

    }

    private void ApplyBounceMaterial(PhysicsMaterial bounceMat)
    {
        GameObject arena = GameObject.Find("Arena");
        if (arena == null) return;

        foreach (Collider col in arena.GetComponentsInChildren<Collider>())
        {
            col.material = bounceMat;
        }
    }
}
