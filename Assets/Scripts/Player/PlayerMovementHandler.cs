using UnityEngine;

public class PlayerMovementHandler : PlatformerMovementHandler {
    [Header("Player Specific Physics Values")]
    [SerializeField] float juggernautMassMultiplier = 3f;
    [SerializeField] float juggernautSpeedMultiplier = 2f;


    // Jugggernaut Logic
    public void JuggernautMovementStart() {
        Body.mass *= juggernautMassMultiplier;
    }

    public void JuggernautMovementUpdateGrounded() {
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

    public void JuggernautMovementUpdateAirborne() {
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

    public void JuggernautMovementEnd() {
        Body.mass /= juggernautMassMultiplier;
    }

}
