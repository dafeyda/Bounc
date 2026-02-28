using System.Collections;
using UnityEngine;

// Attached to the back wall of a goal.
// Increments the appropriate score and resets the ball when contact is made.
public class GoalTrigger : MonoBehaviour
{
    public enum ScoringTarget { Player, Opponent }

    [SerializeField] public ScoringTarget scoringTarget = ScoringTarget.Player;
    [SerializeField] private float launchSpeed   = 3f;
    [SerializeField] private float resetDelay    = 1.5f;
    [SerializeField] private float resetHeight   = 5f;

    private bool _scored = false; // prevents double-counting during the reset delay

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != SceneHelper.BallName) return;
        if (GameHUD.Instance == null) return;
        if (_scored) return;

        _scored = true;

        if (scoringTarget == ScoringTarget.Player)
            GameHUD.Instance.playerScore++;
        else
            GameHUD.Instance.opponentScore++;

        GetComponentInParent<GoalResizer>()?.ResetSize();

        StartCoroutine(ResetBall(collision.gameObject));
    }

    private IEnumerator ResetBall(GameObject ball)
    {
        // Freeze the ball in place during the pause
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 savedVelocity = rb.linearVelocity;
        rb.linearVelocity        = Vector3.zero;
        rb.angularVelocity  = Vector3.zero;
        rb.isKinematic      = true;

        yield return new WaitForSeconds(resetDelay);

        // Re-centre
        ball.transform.position = new Vector3(0f, resetHeight, 0f);
        rb.isKinematic = false;

        // Random direction biased towards the scored side:
        // Player scored → ball heads away from player spawn (+Z / north)
        // Opponent scored → ball heads towards player spawn (-Z / south)
        float zSign = (scoringTarget == ScoringTarget.Player) ? 1f : -1f;
        Vector3 dir = Random.onUnitSphere;
        dir.z = Mathf.Abs(dir.z) * zSign;
        rb.linearVelocity = dir.normalized * launchSpeed;

        _scored = false;
    }
}
