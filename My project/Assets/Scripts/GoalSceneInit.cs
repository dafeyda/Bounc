using UnityEngine;

// Attaches GoalTrigger components to goal back walls at runtime.
// Runs automatically when entering Play mode — no scene object required.
static class GoalSceneInit
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachGoalComponents()
    {
        Attach("Arena/Goal_North/Back", GoalTrigger.ScoringTarget.Player);
        Attach("Arena/Goal_South/Back", GoalTrigger.ScoringTarget.Opponent);

        AttachResizer("Arena/Goal_North");
        AttachResizer("Arena/Goal_South");
    }

    static void Attach(string path, GoalTrigger.ScoringTarget target)
    {
        GameObject go = GameObject.Find(path);
        if (go == null) { Debug.LogWarning($"[GoalSceneInit] Could not find '{path}'"); return; }
        if (go.GetComponent<GoalTrigger>() != null) return;
        go.AddComponent<GoalTrigger>().scoringTarget = target;
    }

    static void AttachResizer(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null) { Debug.LogWarning($"[GoalSceneInit] Could not find '{path}'"); return; }
        if (go.GetComponent<GoalResizer>() != null) return;
        go.AddComponent<GoalResizer>();
    }
}
