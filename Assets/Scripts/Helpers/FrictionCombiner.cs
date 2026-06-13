using UnityEngine;

public static class FrictionCombiner {

    // Determines the effective friction coefficient Unity will use for physics.
    public static float GetCombinedFriction(float friction1, PhysicsMaterialCombine2D frictionCombine1, float friction2, PhysicsMaterialCombine2D frictionCombine2) {
        PhysicsMaterialCombine2D combine = (PhysicsMaterialCombine2D) Mathf.Max((int)frictionCombine1, (int)frictionCombine2);

        // These calculations are pulled straight from Unity's Scripting API.
        switch (combine) {
            case PhysicsMaterialCombine2D.Average:
                return (friction1 + friction2) * 0.5f;
            case PhysicsMaterialCombine2D.Mean:
            default:
                return Mathf.Sqrt(friction1 * friction2);
            case PhysicsMaterialCombine2D.Multiply:
                return friction1 * friction2;
            case PhysicsMaterialCombine2D.Minimum:
                return friction1 < friction2 ? friction1 : friction2;
            case PhysicsMaterialCombine2D.Maximum:
                return friction1 > friction2 ? friction1 : friction2;
            
        }
    }
}