using UnityEngine;

public static class VectorMathHelpers {

    public static Vector2 RotateAround(this Vector2 point, Vector2 pivot, float degrees)
    {
        //Translate point back to origin relative to the pivot
        Vector2 direction = point - pivot;
        
        // Create the rotation quaternion around the Z axis
        Quaternion rotation = Quaternion.Euler(0, 0, degrees);
        
        // Rotate the direction vector and translate it back to the pivot
        return pivot + (Vector2)(rotation * direction);
    }

    public static float GetAngle(this Vector2 point, Vector2 pivot)
    {
        //Translate point back to origin relative to the pivot
        Vector2 direction = point - pivot;
        
        // Create the rotation quaternion around the Z axis
        return Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
    }
}