using UnityEngine;

public static class SceneHelper
{
    public const string BallName = "Ball";

    public static Transform FindBall()
    {
        GameObject ball = GameObject.Find(BallName);
        return ball != null ? ball.transform : null;
    }
}
