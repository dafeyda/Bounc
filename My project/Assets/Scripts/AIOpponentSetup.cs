using UnityEngine;

// Spawns the AI opponent cube at runtime — no scene object required.
// Same pattern as GoalSceneInit.cs.
static class AIOpponentSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SpawnAI()
    {
        // 1. Create primitive (adds MeshFilter, MeshRenderer, BoxCollider)
        GameObject ai = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ai.name = "AIOpponent";

        // 2. Position and scale — matches player: flat paddle (2 × 0.1 × 2)
        ai.transform.position   = new Vector3(0f, 1f, ArenaConstants.HalfDepth - 5f); // (0, 1, 15)
        ai.transform.localScale = new Vector3(2f, 0.1f, 2f);

        // 3. URP Lit material — red-orange so it's distinct from the player
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 0.35f, 0.05f);
        ai.GetComponent<MeshRenderer>().material = mat;

        // 4. Keep the default BoxCollider as-is — the ball's own MaxCombine
        //    bounciness=1 guarantees full reflection off any surface, so no
        //    special material is needed here. (Destroy+re-add was deferred and
        //    briefly left two overlapping colliders, causing double impulses.)

        // 5. CharacterController — matches player (height=0.2, radius=0.1)
        CharacterController cc = ai.AddComponent<CharacterController>();
        cc.height     = 0.2f;
        cc.radius     = 0.1f;
        cc.center     = Vector3.zero;
        cc.stepOffset = 0f;   // must be ≤ height (0.2); AI floats freely, no stepping needed

        // 6. AIOpponent — Awake runs immediately and grabs the CharacterController
        ai.AddComponent<AIOpponent>();
        ai.AddComponent<BallTouchCooldown>();
    }
}
