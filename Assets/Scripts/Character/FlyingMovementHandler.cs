using UnityEngine;

public class FlyingMovementHandler : MovementHandlerBase {

    protected Vector2 nextExpectedVelocity = Vector2.zero;
    protected Vector2 stopLastVelocity = Vector2.zero;
    protected float stopFailTimer = 0.0f;

    public override void ProcessMovementDirection(Vector2 direction) {
        HandleFlyingMovement(direction);
    }

    // My intention is for this to work whether grounded or not:
    public override void ResistMovement() {
        Vector2 desired = Vector2.zero;
        Vector2 delta = desired - Body.linearVelocity;
        Vector2 addedForce = acceleration * delta * Body.mass;

        Body.AddForce(addedForce);
        nextExpectedVelocity = Body.linearVelocity + ((addedForce / Body.mass) * Time.fixedDeltaTime);
        stopFailTimer += Time.fixedDeltaTime;
    }

        // Take the airborne movement code and re-write it in 2D:
    public void HandleFlyingMovement(Vector2 move) {
        // Movement uses rb.AddForce so that environmental physics effects can still work.
        // Horizontal Movement
        Vector2 external_forces = (Body.linearVelocity - nextExpectedVelocity) * Body.mass;

        if (moveControlState == MoveControlState.Stationary || moveControlState == MoveControlState.Stopping) {
            nextExpectedVelocity = Vector2.zero;
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
                stopLastVelocity = Body.linearVelocity;
                // Honestly, not entirely sure why this code causes problems while grounded. 

            } else {
                float strength = 1.0f;
                // We're being acted on by an external force. Allow it to have more noticable effect.
                if (Mathf.Abs(external_forces.magnitude / Body.mass) >= moveResistanceDetectionDifference * Body.mass) {
                    strength = moveeResistanceAccelFraction;
                }

                Vector2 desired = move * moveSpeed;

                // We'll need to write new tailwind code later.

                Vector2 delta = desired - Body.linearVelocity;
                Vector2 addedForce = acceleration * delta * Body.mass * strength;

                Body.AddForce(addedForce, ForceMode2D.Force);
                nextExpectedVelocity = Body.linearVelocity + ((addedForce / Body.mass) * Time.fixedDeltaTime);

            }
        }

        // We were moving, but no longer getting movement direction from the player.
        else if (moveControlState == MoveControlState.Stopping) {
            if (
                Mathf.Abs(Body.linearVelocity.magnitude) <= stopSpeedTarget // Speed at which we're done stopping.
                || Mathf.Abs(stopLastVelocity.magnitude) < Mathf.Abs(Body.linearVelocity.magnitude) //Not slowing down. Must be some some outside force.
                || stopFailTimer >= stopCutoffTime
            ) {
                moveControlState = MoveControlState.Stationary;
            } else {
                Vector2 desired = Vector2.zero;
                Vector2 delta = desired - Body.linearVelocity;
                Vector2 addedForce = acceleration * delta * Body.mass;

                Body.AddForce(addedForce);
                nextExpectedVelocity = Body.linearVelocity + ((addedForce / Body.mass) * Time.fixedDeltaTime);
                stopFailTimer += Time.fixedDeltaTime;
            }
        }
        stopLastVelocity = Body.linearVelocity;
    }
}