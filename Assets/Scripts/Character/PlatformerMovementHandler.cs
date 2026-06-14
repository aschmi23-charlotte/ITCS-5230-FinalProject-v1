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

    [System.Serializable]
    public enum JumpMethod {
        SetVelocity,
        AddForce,
    }
    [Header("Jumping")]
    [SerializeField] protected JumpMethod jumpMethod = JumpMethod.SetVelocity;
    [SerializeField] protected float jumpAcceleration = 10.0f;
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
    public Rigidbody2D Body { get; protected set; }
    public Collider2D Col { get; protected set; }
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
	    Body = GetComponent<Rigidbody2D>();
	    Col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {}

    void FixedUpdate() {
        //RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 0.2f, groundLayers);
        int hit = Col.Cast(Vector2.down, groundFilter, groundedResults, 0.2f);
        IsGrounded = hit > 0;

        if (IsGrounded) {
            CoyoteTimer = 0f;
        } else {
            CoyoteTimer += Time.fixedDeltaTime;
        }

        IsFalling = Body.linearVelocityY < 0.0f;
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

    // This function should ONLY be called if IsGrounded == true
    public void HandleGroundedMovement(Vector2 move) {
        // Using rb.MovePosition or setting rb.linearVelocity every frame results in the player not being affected by in-world physics very much.
        // This movement code uses special physics calulations so that environmental physics effects can still work on the player.
        // In hindsight, I could probably modify this to operate on the velocity only and not AddForce. But why fix what ain't broken?
        // Horizontal Movement
        float external_forces_x = (Body.linearVelocityX - nextExpectedVelocityX) * Body.mass;

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
                // Get applicable friction coefficient:
                float strength = 1.0f;
                // We're being acted on by an external force. Allow it to have more noticable effect.
                if (Mathf.Abs(external_forces_x / Body.mass) >= moveForceDifference * Body.mass) {
                    strength = moveForceResistance;
                }

                float desiredX = move.x * moveSpeed;
                // Don't want slow ourselves down in a tailwind. Use it to go faster.
                if (
                    Body.linearVelocityX != 0.0f
                    && Mathf.Sign(desiredX) == Mathf.Sign(Body.linearVelocityX)
                    && Mathf.Abs(Body.linearVelocityX) > Mathf.Abs(desiredX)
                ) {
                    // Obviously, this will result in the delta being zero. Whatever.
                    desiredX = Body.linearVelocityX;
                }

                float deltaX = desiredX - Body.linearVelocityX;
                float moveForce = acceleration * deltaX * Body.mass * strength;

                float effectiveFriction = FrictionCombiner.GetCombinedFriction(
                    groundedResults[0].collider.friction,
                    groundedResults[0].collider.frictionCombine,
                    Col.friction,
                    Col.frictionCombine
                );

                float frictionForce = moveForce * effectiveFriction;
                float combinedForce = moveForce + frictionForce;
                Body.AddForceX(combinedForce, ForceMode2D.Force);
                nextExpectedVelocityX = Body.linearVelocityX + ((moveForce / Body.mass) * Time.fixedDeltaTime);

                // Newton's third law says for every action there is an equal and opposite reaction.
                // If we have to apply frictionForce to overcome friction pushing the character, then 
                // -frictionForce is required to overcome friction on the floor. This should more 
                // realistically simulate the force on a free-moving floor when someone walks on it.
                if (groundedResults[0].rigidbody != null) {
                    groundedResults[0].rigidbody.AddForceX(-frictionForce, ForceMode2D.Force);
                }
            }
        }

        // We were moving, but no longer getting movement direction from the player.
        else if (moveControlState == MoveControlState.Stopping) {
            if (
                Mathf.Abs(Body.linearVelocityX) <= stopSpeedTarget // Speed at which we're done stopping.
                || Mathf.Sign(stopLastVelocityX) != Mathf.Sign(Body.linearVelocityX) // Edge case, we overshot stopSpeedThreshold in a single tick. 
                || Mathf.Abs(stopLastVelocityX) < Mathf.Abs(Body.linearVelocityX) //Not slowing down. Must be some some outside force.
                || stopFailTimer >= stopCutoffTime
            ) {
                moveControlState = MoveControlState.Stationary;
            } else {
                float desiredX = 0.0f;
                float deltaX = desiredX - Body.linearVelocityX;
                float addedForce = acceleration * deltaX * Body.mass;

                Body.AddForceX(addedForce);
                nextExpectedVelocityX = Body.linearVelocityX + ((addedForce / Body.mass) * Time.fixedDeltaTime);
                stopFailTimer += Time.fixedDeltaTime;
            }
        }

        stopLastVelocityX = Body.linearVelocityX;
    }

    public bool IsCoyoteTimeGrounded() {
        return CoyoteTimer <= coyoteTime;
    }

    public void InitiateJump() {
        switch(jumpMethod) {
            case JumpMethod.SetVelocity:
                Body.linearVelocityY = jumpAcceleration;
                break;
            case JumpMethod.AddForce:
            default:
                Body.AddForceY(jumpAcceleration * Body.mass, ForceMode2D.Impulse);
                break;
        }
    }

    public void HandleJumping(bool shouldCancel) {
        if (shouldCancel && Body.linearVelocityY > 0.0f) {
            // Apply force necessary to arrest
            //rb.AddForceY(jumpAcceleration * rb.mass, ForceMode2D.Impulse);
            Body.linearVelocityY = 0.1f;
        }
    }

    // I keep going back and forth on whether this and the grounded version.
    // I keep telling myself I might end up needing significant differences between the two,
    // and it keeps not happening.
    // EDIT: Nevermind, I guess it happened. At least a little.
    public void HandleAirborneMovement(Vector2 move) {
        // Movement uses rb.AddForce so that environmental physics effects can still work.
        // Horizontal Movement
        float external_forces_x = (Body.linearVelocityX - nextExpectedVelocityX) * Body.mass;

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
                stopLastVelocityX = Body.linearVelocityX;
                // Honestly, not entirely sure why this code causes problems while grounded. 


            } else {
                float strength = 1.0f;
                // We're being acted on by an external force. Allow it to have more noticable effect.
                if (Mathf.Abs(external_forces_x / Body.mass) >= moveForceDifference * Body.mass) {
                    strength = moveForceResistance;
                }

                float desiredX = move.x * moveSpeed;
                // Don't want slow ourselves down in a tailwind. Use it to go faster.
                if (
                    Body.linearVelocityX != 0.0f
                    && Mathf.Sign(desiredX) == Mathf.Sign(Body.linearVelocityX)
                    && Mathf.Abs(Body.linearVelocityX) > Mathf.Abs(desiredX)
                ) {
                    // Obviously, this will result in the delta being zero. Whatever.
                    desiredX = Body.linearVelocityX;
                }

                float deltaX = desiredX - Body.linearVelocityX;
                float addedForce = acceleration * deltaX * Body.mass * strength;
                //Debug.LogFormat("addedForce: {0}", addedForce);
                Body.AddForceX(addedForce, ForceMode2D.Force);
                nextExpectedVelocityX = Body.linearVelocityX + ((addedForce / Body.mass) * Time.fixedDeltaTime);

            }
        }

        // We were moving, but no longer getting movement direction from the player.
        else if (moveControlState == MoveControlState.Stopping) {
            if (
                Mathf.Abs(Body.linearVelocityX) <= stopSpeedTarget // Speed at which we're done stopping.
                || Mathf.Sign(stopLastVelocityX) != Mathf.Sign(Body.linearVelocityX) // Edge case, we overshot stopSpeedThreshold in a single tick. 
                || Mathf.Abs(stopLastVelocityX) < Mathf.Abs(Body.linearVelocityX) //Not slowing down. Must be some some outside force.
                || stopFailTimer >= stopCutoffTime
            ) {
                moveControlState = MoveControlState.Stationary;
            } else {
                float desiredX = 0.0f;
                float deltaX = desiredX - Body.linearVelocityX;
                float addedForce = acceleration * deltaX * Body.mass;

                Body.AddForceX(addedForce);
                nextExpectedVelocityX = Body.linearVelocityX + ((addedForce / Body.mass) * Time.fixedDeltaTime);
                stopFailTimer += Time.fixedDeltaTime;
            }
        }

        stopLastVelocityX = Body.linearVelocityX;
    }
}
