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
    public Vector2 GetAimDirection() {
        Vector2 direction = InputReader.ReadMoveInput();
        if (direction == Vector2.zero) {
            return Movement.GetFacingAsVector();
        }
        return direction;
    }

    public void UpdateAim() {
        Weapons.SetAimByDirection(GetAimDirection());
    }


    public Vector2 GetMovementDirection(bool considerFreeAim) {

        if (considerFreeAim && InputReader.CheckFreeAimInput(PlayerInputReader.ButtonCheckType.Held)) {
            return Vector2.zero;
        }

        return InputReader.ReadMoveInput();
    }

    // === Abilities ===
    // Juggernaut
    public void JuggernautStart() {
        spriteRenderer.color = juggernautColor;
        Movement.JuggernautMovementStart();
    }

    public void JuggernautUpdateGrounded() {
        Movement.JuggernautMovementUpdateGrounded();
    }

    public void JuggernautUpdateAirborne() {
        Movement.JuggernautMovementUpdateAirborne();
    }

    public void JuggernautEnd() {
        spriteRenderer.color = baseColor;
        Movement.JuggernautMovementEnd();
    }
}
