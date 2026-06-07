using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputReader : MonoBehaviour {

    public GameObject crosshairPrefab;

    [System.Serializable]
    public enum ButtonCheckType {
        Pressed,
        Released,
        Held
    }

    // Because so much of the player code is handled on FixedUpdate, I need to be able to 
    // Check input events on FixedUpdate too... And that doesn't work well out of the box.
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


    [System.Serializable]
    public enum ControlScheme {
        KeyboardAndMouse,
        Gamepad
    }
    public ControlScheme CurrentScheme { get; private set; } = ControlScheme.Gamepad;

    // Attributes
    // Is this particularly DRY? No. But it should be performant, especially
    // considering most of this information is accessed via visual scripting.
    // I want to minimize graph traversal for the player states.
    public InputAction MoveInput { get; private set; }
    public InputAction JumpInput { get; private set; }
    public InputAction ClampInput { get; private set; }
    public InputAction DistortionInput { get; private set; }
    public InputAction JuggernautInput { get; private set; }
    public InputAction PrimaryFireInput { get; private set; }
    public InputAction SecondaryFireInput { get; private set; }
    public InputAction FreeAimInput { get; private set; }
    public InputAction InteractInput { get; private set; }
    public InputAction SelectWeapon1Input { get; private set; }
    public InputAction SelectWeapon2Input { get; private set; }
    public InputAction SelectWeapon3Input { get; private set; }
    public InputAction SelectWeapon4Input { get; private set; }
    
    public ButtonScanner JumpScanner { get; private set; }
    public ButtonScanner ClampScanner { get; private set; }
    public ButtonScanner DistortionScanner { get; private set; }
    public ButtonScanner JuggernautScanner { get; private set; }
    public ButtonScanner PrimaryFireScanner { get; private set; }
    public ButtonScanner SecondaryFireScanner { get; private set; }
    public ButtonScanner FreeAimScanner { get; private set; }
    public ButtonScanner InteractScanner { get; private set; }
    public ButtonScanner SelectWeapon1Scanner { get; private set; }
    public ButtonScanner SelectWeapon2Scanner { get; private set; }
    public ButtonScanner SelectWeapon3Scanner { get; private set; }
    public ButtonScanner SelectWeapon4Scanner { get; private set; }

    public Vector2 MouseAimPos { get; private set; }
    public GameObject Crosshair { get; private set; }

    void Awake() {
        MoveInput = InputSystem.actions.FindAction("Move");
        JumpInput = InputSystem.actions.FindAction("Jump");
        ClampInput = InputSystem.actions.FindAction("Clamp");
        DistortionInput = InputSystem.actions.FindAction("Distortion");
        JuggernautInput = InputSystem.actions.FindAction("Juggernaut");
        PrimaryFireInput = InputSystem.actions.FindAction("Primary Fire");
        SecondaryFireInput = InputSystem.actions.FindAction("Secondary Fire");
        FreeAimInput = InputSystem.actions.FindAction("Free Aim");
        InteractInput = InputSystem.actions.FindAction("Interact");
        SelectWeapon1Input = InputSystem.actions.FindAction("Select Weapon 1");
        SelectWeapon2Input = InputSystem.actions.FindAction("Select Weapon 2");
        SelectWeapon3Input = InputSystem.actions.FindAction("Select Weapon 3");
        SelectWeapon4Input = InputSystem.actions.FindAction("Select Weapon 4");

        JumpScanner = new ButtonScanner(JumpInput);
        ClampScanner = new ButtonScanner(ClampInput);
        DistortionScanner = new ButtonScanner(DistortionInput);
        JuggernautScanner = new ButtonScanner(JuggernautInput);
        PrimaryFireScanner = new ButtonScanner(PrimaryFireInput);
        SecondaryFireScanner = new ButtonScanner(SecondaryFireInput);
        FreeAimScanner = new ButtonScanner(FreeAimInput);
        InteractScanner = new ButtonScanner(InteractInput);
        SelectWeapon1Scanner = new ButtonScanner(SelectWeapon1Input);
        SelectWeapon2Scanner = new ButtonScanner(SelectWeapon2Input);
        SelectWeapon3Scanner = new ButtonScanner(SelectWeapon3Input);
        SelectWeapon4Scanner = new ButtonScanner(SelectWeapon4Input);

        Crosshair = null;
        MouseAimPos = Vector2.zero;
    }

    void FixedUpdate() {
        JumpScanner.Poll();
        ClampScanner.Poll();
        DistortionScanner.Poll();
        JuggernautScanner.Poll();
        PrimaryFireScanner.Poll();
        SecondaryFireScanner.Poll();
        FreeAimScanner.Poll();
        InteractScanner.Poll();
        SelectWeapon1Scanner.Poll();
        SelectWeapon2Scanner.Poll();
        SelectWeapon3Scanner.Poll();
        SelectWeapon4Scanner.Poll();

        if (CurrentScheme == ControlScheme.KeyboardAndMouse && Crosshair != null) {
            if (Mouse.current != null) {
                //Read Vector2 screen coordinates from the active mouse
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

                //Convert to world position using the main camera
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                mouseWorldPos.z = 0f;

                MouseAimPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                Crosshair.transform.position = mouseWorldPos;
            }
        }
    }

    public Vector2 ReadMoveInput() {
        return MoveInput.ReadValue<Vector2>();
    }

    public bool CheckJumpInput(ButtonCheckType checkType) {
        return JumpScanner.Check(checkType);
    }
    public bool CheckClampInput(ButtonCheckType checkType) {
        return ClampScanner.Check(checkType);
    }
    public bool CheckDistortionInput(ButtonCheckType checkType) {
        return DistortionScanner.Check(checkType);
    }
    public bool CheckJuggernautInput(ButtonCheckType checkType) {
        return JuggernautScanner.Check(checkType);
    }
    public bool CheckPrimaryFireInput(ButtonCheckType checkType) {
        return PrimaryFireScanner.Check(checkType);
    }
    public bool CheckSecondaryFirInput(ButtonCheckType checkType) {
        return SecondaryFireScanner.Check(checkType);
    }
    public bool CheckFreeAimInput(ButtonCheckType checkType) {
        return FreeAimScanner.Check(checkType);
    }
    public bool CheckInteractInput(ButtonCheckType checkType) {
        return InteractScanner.Check(checkType);
    }
    public bool CheckSelectWeapon1Input(ButtonCheckType checkType) {
        return SelectWeapon1Scanner.Check(checkType);
    }
    public bool CheckSelectWeapon2Input(ButtonCheckType checkType) {
        return SelectWeapon2Scanner.Check(checkType);
    }
    public bool CheckSelectWeapon3Input(ButtonCheckType checkType) {
        return SelectWeapon3Scanner.Check(checkType);
    }
    public bool CheckSelectWeapon4Input(ButtonCheckType checkType) {
        return SelectWeapon4Scanner.Check(checkType);
    }

    // Handle changing the player's input mode:
    private void OnEnable() {
        // Hook into the global action change event listener
        InputSystem.onActionChange += OnGlobalActionChange;
    }

    private void OnDisable() {
        InputSystem.onActionChange -= OnGlobalActionChange;
    }

    // Here we handle switching between KB+M and Gamepad input stuff.
    private void OnGlobalActionChange(object actionOrMapOrAsset, InputActionChange change) {
        // We only care when an action is actively performed
        if (change != InputActionChange.ActionStarted && change != InputActionChange.ActionPerformed)
            return;

        if (actionOrMapOrAsset is InputAction action) {
            // Get the device that triggered this specific action
            InputDevice device = action.activeControl?.device;
            if (device == null) return;

            ControlScheme detectedScheme = CurrentScheme;

            // Check the device type using standard Input System classes
            if (device is Keyboard || device is Mouse) {
                detectedScheme = ControlScheme.KeyboardAndMouse;
            } else if (device is Gamepad) {
                detectedScheme = ControlScheme.Gamepad;
            }

            // Only trigger updates if the control scheme actually switched
            if (detectedScheme != CurrentScheme) {
                CurrentScheme = detectedScheme;
                Debug.Log($"[GlobalInput] Control scheme switched to: {CurrentScheme}");
                if (CurrentScheme == ControlScheme.KeyboardAndMouse) {
                    SwitchToKeyboardAndMouse();
                } else if (CurrentScheme == ControlScheme.Gamepad) {
                    SwitchToGamepad();
                }
            }
        }
    }

    private void SwitchToKeyboardAndMouse() {
        Crosshair = Instantiate(crosshairPrefab);
        Cursor.visible = false;
    }

    private void SwitchToGamepad() {
        Destroy(Crosshair);
        Crosshair = null;
        Cursor.visible = true;
    }

}
