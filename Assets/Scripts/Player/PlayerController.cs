using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [Header("Horizontal Movement")]
    // Target move speed for the player.
    public float moveSpeed = 10.0f;
    // The Player's acceleration
    public float acceleration = 7.0f;

    [Header("Jumping")]
    public float jumpAcceleration = 10.0f;
    public LayerMask groundLayers;

    [System.Serializable]
    public enum JumpControlState {
        NotJumping,
        Start,
        Held,
        Released
    }
    public JumpControlState jumpControlState = JumpControlState.NotJumping;

    [Header("Movement Physics")]
    public float moveActionDeadzone = 0.2f;
    public float moveForceDifference = 0.3f;
    public float moveForceResistance = 0.3f;
    public float stopSpeedTarget = 0.0f;
    public float stopCutoffTime = 0.5f;

    [System.Serializable]
    public enum MoveControlState {
        Stationary,
        Moving,
        Stopping,
    }
    public MoveControlState moveControlState = MoveControlState.Stationary;


    // Components
    public static PlayerController Instance { get; private set; }

    public SpriteRenderer render { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D col { get; private set; }

    // Public fields 
    public bool isGrounded { get; private set; }

    // Protected Fields
    protected InputAction moveInput = null;
    protected InputAction jumpInput = null;

    // Used to determine if outside forces are affecting the rigidbody.
    protected float nextExpectedVelocityX = 0.0f;
    protected float stopLastVelocityX = 0.0f;
    protected float stopFailTimer = 0.0f;

    void Awake() {
        Debug.Assert(Instance == null, "Multiple instances of GlobalController present!");
        if (Instance == null) {
            Instance = this;
            Debug.Log("GlobalController instance awakened.");
        }

        isGrounded = false;
    }

    // Unity Events:
    void OnEnable() {
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
    }

    private void OnDisable() {}

    void Start() {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        if (isGrounded && jumpInput.WasPressedThisFrame()) {
            jumpControlState = JumpControlState.Start;

        } else if (jumpControlState == JumpControlState.Held && jumpInput.WasReleasedThisFrame()) {
            jumpControlState = JumpControlState.Released;

        }
    }

    void FixedUpdate() {
        CheckGrounded();
        HandleHorizontalMovement();
	    HandleJumping();
    }

    void CheckGrounded() {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 0.2f, groundLayers);
        isGrounded = (bool)hit;
    }

    void HandleHorizontalMovement() {
        // Movement uses rb.AddForce so that environmental physics effects can still work.

        // Horizontal Movement
        Vector2 move = moveInput.ReadValue<Vector2>();
        float external_forces_x = (rb.linearVelocityX - nextExpectedVelocityX) * rb.mass;

        if (moveControlState == MoveControlState.Stationary || moveControlState == MoveControlState.Stopping) {
            nextExpectedVelocityX = 0.0f;
            if (move.magnitude > moveActionDeadzone) {
                moveControlState = MoveControlState.Moving;
            }
        }

        // Player is saying to move.
        if (moveControlState == MoveControlState.Moving) {
            // Transition to stopping if the input stops:
            if (move.magnitude <= moveActionDeadzone) {
                moveControlState = MoveControlState.Stopping;
                stopFailTimer = 0.0f; // Reset the cutoff timer.

                if (!isGrounded) {
                    // While airborne, prevents the stop code from seeing acceleration from
                    // the movement state and bailing out incorrectly from that.
                    stopLastVelocityX = rb.linearVelocityX;
                    // Honestly, not entirely sure why this causes problems while grounded. 
                }


            } else {
                float strength = 1.0f;
                // We're being acted on by an external force. Allow it to have more noticable effect.
                if (Mathf.Abs(external_forces_x / rb.mass) >= moveForceDifference * rb.mass) {
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
                float addedForce = acceleration * deltaX * rb.mass * strength;
                //Debug.LogFormat("addedForce: {0}", addedForce);
                rb.AddForceX(addedForce, ForceMode2D.Force);
                nextExpectedVelocityX = rb.linearVelocityX + ((addedForce / rb.mass) * Time.fixedDeltaTime);

            }
        }

        // We were moving, but no longer getting movement direction from the player.
        else if (moveControlState == MoveControlState.Stopping) {
            if (
                Mathf.Abs(rb.linearVelocityX) <= stopSpeedTarget // Speed at which we're done stopping.
                || Mathf.Sign(stopLastVelocityX) != Mathf.Sign(rb.linearVelocityX) // Edge case, we overshot stopSpeedThreshold in a single tick. 
                || Mathf.Abs(stopLastVelocityX) < Mathf.Abs(rb.linearVelocityX) //Not slowing down. Must be some some outside force.
                || stopFailTimer >= stopCutoffTime
            ) {
                moveControlState = MoveControlState.Stationary;
            } else {
                float desiredX = 0.0f;
                float deltaX = desiredX - rb.linearVelocityX;
                float addedForce = acceleration * deltaX * rb.mass;

                rb.AddForceX(addedForce);
                nextExpectedVelocityX = rb.linearVelocityX + ((addedForce / rb.mass) * Time.fixedDeltaTime);
                stopFailTimer += Time.fixedDeltaTime;
            }
        }

        stopLastVelocityX = rb.linearVelocityX;
        //Debug.LogFormat("rb.linearVelocityX: {0}", rb.linearVelocityX);
        Debug.LogFormat("external_forces_x: {0}", external_forces_x);
    }


    void HandleJumping() {
        // Jumping
        if (jumpControlState == JumpControlState.Start) {
            // Initial jump impulse:
            rb.AddForceY(jumpAcceleration * rb.mass, ForceMode2D.Impulse);
            jumpControlState = JumpControlState.Held;

        } else if (jumpControlState == JumpControlState.Held && rb.linearVelocityY <= 0.0f) {
            // Apex of jump reached. We're falling now.
            jumpControlState = JumpControlState.NotJumping;

        } else if (jumpControlState == JumpControlState.Released && rb.linearVelocityY > 0.0f) {
            // Apply force necessary to arrest
            //rb.AddForceY(jumpAcceleration * rb.mass, ForceMode2D.Impulse);
            rb.linearVelocityY = 0.1f;
            jumpControlState = JumpControlState.NotJumping;
        }
    }
}