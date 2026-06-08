using System.Collections;
using UnityEngine;

public class PlatformerMovementHandler : MonoBehaviour {
    // Editor Fields
    [Header("Horizontal Movement")]
    // Target move speed for the player.
    [SerializeField] protected float moveSpeed = 10.0f;
    // The Player's acceleration
    [SerializeField] protected float acceleration = 7.0f;
    
    [System.Serializable]
    public enum FacingDirection {
        Left,
        Right,
    }
    public FacingDirection facingDirection = FacingDirection.Right;

    [Header("Jumping")]
    [SerializeField] protected float jumpAcceleration = 10.0f;
    //public LayerMask groundLayers;
    [SerializeField] protected ContactFilter2D groundFilter;
    [SerializeField] protected float coyoteTime = 0.2f;

    [Header("Physics Values")]
    [SerializeField] protected float moveActionDeadzone = 0.2f;
    [SerializeField] protected float moveForceDifference = 0.3f;
    [SerializeField] protected float moveForceResistance = 0.3f;
    [SerializeField] protected float stopSpeedTarget = 0.0f;
    [SerializeField] protected float stopCutoffTime = 0.5f;

    [System.Serializable]
    public enum MoveControlState {
        Stationary,
        Moving,
        Stopping,
    }
    [SerializeField] MoveControlState moveControlState = MoveControlState.Stationary;

    // Attributes
    public Rigidbody2D rb { get; protected set; }
    public Collider2D col { get; protected set; }
    public bool IsGrounded { get; protected set; }
    public float CoyoteTimer { get; protected set; }
    public bool IsFalling { get; protected set; }

    // Internal values
    protected float nextExpectedVelocityX = 0.0f;
    protected float stopLastVelocityX = 0.0f;
    protected float stopFailTimer = 0.0f;
    private RaycastHit2D[] groundedResults = new RaycastHit2D[1];

    // === Methods ===
    void Awake() {
        IsGrounded = false;
        IsFalling = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {}

    void FixedUpdate() {
        //RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 0.2f, groundLayers);
        int hit = col.Cast(Vector2.down, groundFilter, groundedResults, 0.2f);
        IsGrounded = hit > 0;

        if (IsGrounded) {
            CoyoteTimer = 0f;
        } else {
            CoyoteTimer += Time.fixedDeltaTime;
        }

        IsFalling = rb.linearVelocityY < 0.0f;
    }

    public void UpdateFacing(Vector2 direction) {
        if (direction.x > 0f) {
            facingDirection = FacingDirection.Right;
        } else if (direction.x < 0f) {
            facingDirection = FacingDirection.Left;
        }
    }

    public Vector2 GetFacingAsVector() {
        switch (facingDirection) {
            case FacingDirection.Left:
                return Vector2.left;
            case FacingDirection.Right:
            default:
                return Vector2.right;
        }
    }

    public RaycastHit2D GetGroundedCast() {
        return groundedResults[0];
    }

    public void HandleGroundedMovement(Vector2 move) {
        // Movement uses rb.AddForce so that environmental physics effects can still work.

        // Horizontal Movement
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
    }

    public bool IsCoyoteTimeGrounded() {
        return CoyoteTimer <= coyoteTime;
    }

    public void InitiateJump() {
        rb.AddForceY(jumpAcceleration * rb.mass, ForceMode2D.Impulse);

    }

    public void HandleJumping(bool shouldCancel) {
        if (shouldCancel && rb.linearVelocityY > 0.0f) {
            // Apply force necessary to arrest
            //rb.AddForceY(jumpAcceleration * rb.mass, ForceMode2D.Impulse);
            rb.linearVelocityY = 0.1f;
        }
    }

    // I keep going back and forth on whether this and the grounded version.
    // I keep telling myself I might end up needing significant differences between the two,
    // and it keeps not happening.
    public void HandleAirborneMovement(Vector2 move) {
        // Movement uses rb.AddForce so that environmental physics effects can still work.

        // Horizontal Movement
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


                // While airborne, prevents the stop code from seeing acceleration from
                // the movement state and bailing out incorrectly from that.
                stopLastVelocityX = rb.linearVelocityX;
                // Honestly, not entirely sure why this causes problems while grounded. 


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
    }
}
