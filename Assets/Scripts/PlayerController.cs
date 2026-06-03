using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour {
    [Header("Horizontal Movement")]
    // Target move speed for the player.
    public float moveSpeed = 10.0f;
    // The Player's acceleration
    public float acceleration = 7.0f;
    public float moveActionDeadzone = 0.2f;
    public float moveDiffThreshold = 0.11f;
    public float moveForceResistance = 0.4f;
    public float stopSpeedThreshold = 0.1f;
    public float stopDiffThreshold = 0.1f;
    [System.Serializable]
    public enum MoveControlState {
        Stationary,
        Moving,
        Stopping,
    }
    public MoveControlState moveControlState = MoveControlState.Stationary;

    [Header("Jumping")]
    public float jumpForce = 10.0f;

    // Components
    public static PlayerController Instance { get; private set; }

    public SpriteRenderer render { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D col { get; private set; }

    // Protected Fields
    protected InputAction moveInput = null;
    protected InputAction jumpInput = null;

    protected bool processJump = false;
    // Used to determine if outside forces are affecting the rigidbody.
    protected float nextExpectedVelocityX = 0.0f;
    protected float stopLastVelocityX = 0.0f;

    void Awake() {
        Debug.Assert(Instance == null, "Multiple instances of GlobalController present!");
        if (Instance == null) {
            Instance = this;
            Debug.Log("GlobalController instance awakened.");
        }
    }

    // Input Events:
    private void OnJumpPressed(InputAction.CallbackContext context) {
        processJump = true;
    } 

    // Unity Events:
    void OnEnable() {
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");

        jumpInput.started += OnJumpPressed;
    }

    private void OnDisable() {
        jumpInput.started -= OnJumpPressed;
    }

    void Start() {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        HandlePlayerMovement();
    }

    void HandlePlayerMovement() {
        // Movement has fancy uses of rb.AddForce so that environmental physics effects can still work.
        // You know, since I'm doing external force detection using rb.linearVelocityX,
        // I could probably rewrite this to set linearVelocityX directly for movement.
        // Eh. Why fix what ain't broken?

        // Horizontal Movement
        Vector2 move = moveInput.ReadValue<Vector2>();

        if (moveControlState == MoveControlState.Stationary || moveControlState == MoveControlState.Stopping) {
            if (move.magnitude > moveActionDeadzone) {
                moveControlState = MoveControlState.Moving;
            }
        }

        // Player is saying to move.
        if (moveControlState == MoveControlState.Moving) {
            // Transition to stopping if the input stops:
            if (move.magnitude < moveActionDeadzone) {
                moveControlState = MoveControlState.Stopping;

            } else {
                float strength = 1.0f;
                // We're being acted on by an external force. Allow it to have more noticable effect.
                if (Mathf.Abs(rb.linearVelocityX - nextExpectedVelocityX) > moveDiffThreshold) {
                    strength = moveForceResistance;
                }

                float desiredX = move.x * moveSpeed;
                // Don't want slow ourselves down in a tailwind. Use it to go faster.
                if (
                    rb.linearVelocityX != 0.0f 
                    && Mathf.Sign(desiredX) == Mathf.Sign(rb.linearVelocityX)
                    && Mathf.Abs(rb.linearVelocityX) > Mathf.Abs(desiredX)
                ) {
                    // Obviously, this will result in the delta being zero. Whatever.
                    desiredX = rb.linearVelocityX;
                }

                float deltaX = desiredX - rb.linearVelocityX;
                float addedForce = acceleration * deltaX;
                //Debug.LogFormat("addedForce: {0}", addedForce);
                rb.AddForceX(addedForce * strength, ForceMode2D.Force);
                nextExpectedVelocityX = rb.linearVelocityX + ((addedForce / rb.mass) * Time.fixedDeltaTime);

            }
        }

        // We were moving, but no longer getting movement direction from the player.
        if (moveControlState == MoveControlState.Stopping) {
            if (
                Mathf.Abs(rb.linearVelocityX) < stopSpeedThreshold // Speed at which we're done stopping.
                || Mathf.Sign(stopLastVelocityX) != Mathf.Sign(rb.linearVelocityX) // Edge case, we overshot stopSpeedThreshold in a single tick. 
                || Mathf.Abs(rb.linearVelocityX - nextExpectedVelocityX) > stopDiffThreshold // We were affected by some outside force.
            ) {
                moveControlState = MoveControlState.Stationary;
            } else {
                float desiredX = 0.0f;
                float deltaX = desiredX - rb.linearVelocityX;
                float addedForce = acceleration * deltaX;

                rb.AddForceX(addedForce);
                nextExpectedVelocityX = rb.linearVelocityX + ((addedForce / rb.mass) * Time.fixedDeltaTime);
            }
        }

        stopLastVelocityX = rb.linearVelocityX;

        Debug.LogFormat("rb.linearVelocityX: {0}", rb.linearVelocityX);

        // Jumping
        if (processJump) {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        }
        processJump = false;
    }
}