using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputReader : MonoBehaviour {

    [System.Serializable]
    public enum ButtonCheckType {
        Pressed,
        Released,
        Held
    }

    public class ButtonScanner {

        public bool Pressed { get; private set; }
        public bool Released { get; private set; }
        public bool WasHeld { get; private set; }
        public bool Held { get; private set; }

        protected InputAction action = null;

        public ButtonScanner(InputAction p_action) {
            Pressed = false;
            Released = false;
            Held = false;
            WasHeld = false;

            action = p_action;
        }

        public void Poll() {
            WasHeld = Held;
            Held = action.IsPressed();
            Pressed = Held && !WasHeld;
            Released = (!Held) && WasHeld;
        }

        public bool Check(ButtonCheckType btype) {
            switch (btype) {
                case ButtonCheckType.Pressed:
                    return Pressed;
                case ButtonCheckType.Released:
                    return Released;
                case ButtonCheckType.Held:
                default:
                    return Held;
            }
        }
    }

    // Fields
    public InputAction MoveInput { get; private set; }
    public InputAction JumpInput { get; private set; }
    public ButtonScanner JumpScanner { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        MoveInput = InputSystem.actions.FindAction("Move");
        JumpInput = InputSystem.actions.FindAction("Jump");

        JumpScanner = new ButtonScanner(JumpInput);
    }

    void FixedUpdate() {
        JumpScanner.Poll();
    }

    public Vector2 ReadMoveInput() {
        return MoveInput.ReadValue<Vector2>();
    }

    public bool JumpInputCheck(ButtonCheckType checkType) {
        return JumpScanner.Check(checkType);
    }
}
