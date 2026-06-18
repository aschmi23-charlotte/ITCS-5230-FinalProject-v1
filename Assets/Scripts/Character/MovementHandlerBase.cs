using UnityEngine;

public class MovementHandlerBase : MonoBehaviour {
    [Header("Movement Values")]
    // Target move speed for the player.
    [SerializeField] protected float moveSpeed = 15.0f;
    // The Player's acceleration
    [SerializeField] protected float acceleration = 12.0f;
    [SerializeField] protected float resistStateAcceleration = 10.0f;
    
    [System.Serializable]
    public enum FacingDirection {
        Left,
        Right,
    }
    public FacingDirection facingDirection = FacingDirection.Right;

    [Header("Other Physics Values")]
    [SerializeField] protected float moveActionDeadzone = 0.2f;
    [SerializeField] protected float moveResistanceDetectionDifference = 0.3f;
    [SerializeField] protected float moveeResistanceAccelFraction = 0.3f;
    [SerializeField] protected float stopSpeedTarget = 0.0f;
    [SerializeField] protected float stopCutoffTime = 0.5f; 


    [System.Serializable]
    public enum MoveControlState {
        Stationary,
        Moving,
        Stopping,
    }
    [Header("Movement State Handling")]
    [SerializeField] protected MoveControlState moveControlState = MoveControlState.Stationary;

    // Attributes:
    public Rigidbody2D Body { get; protected set; }
    public Collider2D Col { get; protected set; }
    protected float nextExpectedVelocityX = 0.0f;
    protected float stopLastVelocityX = 0.0f;
    protected float stopFailTimer = 0.0f;

    protected virtual void Awake() {
        Body = GetComponent<Rigidbody2D>();
	    Col = GetComponent<Collider2D>();
    }

    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}

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
/*
    public void HandleBaseMovement(Vector2 move) {
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
                if (Mathf.Abs(external_forces_x / Body.mass) >= moveResistanceDetectionDifference * Body.mass) {
                    strength = moveeResistanceAccelFraction;
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
*/
    // My intention is for this to work whether grounded or not:
    public void ResistMovement() {
        float desiredX = 0.0f; // stopSpeedTarget is supposed to get us as close to this as possible. I should probably just hard-code that.
        float deltaX = desiredX - Body.linearVelocityX;
        float stopForce = resistStateAcceleration * deltaX * Body.mass;

        Body.AddForceX(stopForce);
        nextExpectedVelocityX = Body.linearVelocityX + ((stopForce / Body.mass) * Time.fixedDeltaTime);
    }
}