using Unity.VisualScripting;
using UnityEngine;
using static PlatformerMovementHandler;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovementHandler))]
[RequireComponent(typeof(WeaponSystem))]
[RequireComponent(typeof(Variables))]
[RequireComponent(typeof(StateMachine))]
public class PlayerBrain : MonoBehaviour {
    // This file is mostly here to tie all the discrete pieces of the player together.

    // Editor Fields
    [Header("Player Sprite")]
    public SpriteRenderer spriteRenderer = null;
    public Color baseColor = new Color(1f, 1f, 1f, 1f);
    public Color juggernautColor = new Color(1f, 0f, 0f, 1f);

    // Attributes
    public PlayerInputReader InputReader { get; private set; }
    public PlayerMovementHandler Movement { get; private set; }
    public WeaponSystem Weapons { get; private set; }
    public Variables StateVariables { get; private set; }
    public StateMachine StateMachine { get; private set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        InputReader = GetComponent<PlayerInputReader>();
        Movement = GetComponent<PlayerMovementHandler>();
        Weapons = GetComponent<WeaponSystem>();
        StateVariables = GetComponent<Variables>();
        StateMachine = GetComponent<StateMachine>();
    }

    void Update() { }



    // Movement and Aiming Inputs
    public Vector2 GetGamepadAimDirection() {
        Vector2 direction = InputReader.ReadMoveInput();
        if (direction == Vector2.zero) {
            return Movement.GetFacingAsVector();
        }
        return direction;
    }

    public void UpdateAim() {
        if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.Gamepad) {
            Weapons.SetAimByDirection(GetGamepadAimDirection());
        }
        else if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.KeyboardAndMouse) {
            Weapons.SetAimByPosition(InputReader.MouseAimPos);
        }
    }


    public Vector2 GetMovementDirection(bool considerFreeAim) {

        if (considerFreeAim && InputReader.CheckFreeAimInput(PlayerInputReader.ButtonCheckType.Held)) {
            return Vector2.zero;
        }

        return InputReader.ReadMoveInput();
    }

    // === State Functions ===
    // All of these functions are called by the Visual Scripting State machine.
    // Normal Behavior
    public void State_NormalUpdateGrounded() {
        if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.Gamepad) {
            Vector2 dir = GetMovementDirection(true);
            Movement.UpdateFacing(dir);
            Movement.HandleGroundedMovement(dir);

        } else if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.KeyboardAndMouse) {
            Vector2 dir = GetMovementDirection(false);
            Movement.UpdateFacing(InputReader.MouseAimPos - (Vector2)transform.position);
            Movement.HandleGroundedMovement(dir);
        }
    }

    public void State_NormalUpdateJumping() {
        bool shouldCancel = InputReader.CheckJumpInput(PlayerInputReader.ButtonCheckType.Released);
        Movement.HandleJumping(shouldCancel);

        State_NormalUpdateAirborne();
    }

    public void State_NormalUpdateAirborne() {
        if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.Gamepad) {
            Vector2 dir = GetMovementDirection(true);
            Movement.UpdateFacing(dir);
            Movement.HandleAirborneMovement(dir);
        
        } else if (InputReader.CurrentScheme == PlayerInputReader.ControlScheme.KeyboardAndMouse) {
            Vector2 dir = GetMovementDirection(false);
            Movement.UpdateFacing(InputReader.MouseAimPos - (Vector2)transform.position);
            Movement.HandleAirborneMovement(dir);
        }
    }

    // Juggernaut
    public void State_JuggernautStart() {
        spriteRenderer.color = juggernautColor;
        Movement.JuggernautMovementStart();
    }

    public void State_JuggernautUpdateGrounded() {
        Vector2 dir = InputReader.ReadMoveInput();
        Movement.UpdateFacing(dir);
        Movement.JuggernautMovementUpdateGrounded();
    }

    public void State_JuggernautUpdateJumping() {
        bool shouldCancel = InputReader.CheckJumpInput(PlayerInputReader.ButtonCheckType.Released);
        Movement.HandleJumping(shouldCancel);

        State_JuggernautUpdateAirborne();
    }

    public void State_JuggernautUpdateAirborne() {
        Vector2 dir = InputReader.ReadMoveInput();
        Movement.UpdateFacing(dir);
        Movement.JuggernautMovementUpdateAirborne();
    }

    public void State_JuggernautEnd() {
        spriteRenderer.color = baseColor;
        Movement.JuggernautMovementEnd();
    }
}
