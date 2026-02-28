using UnityEngine;

public static class PhysicsHelper
{
    // Creates a perfectly bouncy, frictionless physics material.
    // Used by the ball and the player collider so both surfaces behave consistently.
    public static PhysicsMaterial CreateBouncyMaterial(string name = "Bouncy")
    {
        PhysicsMaterial mat = new PhysicsMaterial(name);
        mat.bounciness      = 1f;
        mat.dynamicFriction = 0f;
        mat.staticFriction  = 0f;
        mat.bounceCombine   = PhysicsMaterialCombine.Maximum;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        return mat;
    }
}
