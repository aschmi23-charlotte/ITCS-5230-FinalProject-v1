using UnityEngine;

public class PlayerMovementHandler : PlatformerMovementHandler {
    [Header("Player Specific Physics Values")]
    public float juggernautMassMultiplier = 3f;
    public float juggernautSpeedMultiplier = 2f;


    // Jugggernaut Logic
    public void JuggernautStart() {
        rb.mass *= juggernautMassMultiplier;
    }

    public void JuggernautUpdateGrounded() {
        switch(facingDirection) {
            case FacingDirection.Left:
                HandleGroundedMovement(Vector2.left * juggernautSpeedMultiplier);
                break;
            case FacingDirection.Right:
            default:
                HandleGroundedMovement(Vector2.right * juggernautSpeedMultiplier);
                break;
        }
    }

    public void JuggernautUpdateAirborne() {
        switch (facingDirection) {
            case FacingDirection.Left:
                HandleAirborneMovement(Vector2.left * juggernautSpeedMultiplier);
                break;
            case FacingDirection.Right:
            default:
                HandleAirborneMovement(Vector2.right * juggernautSpeedMultiplier);
                break;
        }
    }

    public void JuggernautEnd() {
        rb.mass /= juggernautMassMultiplier;
    }

}
