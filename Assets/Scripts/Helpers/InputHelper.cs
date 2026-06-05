using UnityEngine;
using UnityEngine.InputSystem;

public class InputHelpers : MonoBehaviour {
    public static Vector2 ReadVector2(InputAction action) {
        return action.ReadValue<Vector2>();
    }

    public static float ReadFloat(InputAction action) {
        return action.ReadValue<float>();
    }

    public static bool IsPressed(InputAction action) {
        return action.IsPressed();
    }
}