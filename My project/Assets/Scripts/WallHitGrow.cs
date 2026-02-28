using UnityEngine;

// Attached at runtime to the arena wall sections surrounding a goal.
// When the ball strikes this wall the associated goal opening grows,
// rewarding shots that hit the wall near the goal.
public class WallHitGrow : MonoBehaviour
{
    public GoalResizer goalResizer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != SceneHelper.BallName) return;
        goalResizer?.Grow();
    }
}
