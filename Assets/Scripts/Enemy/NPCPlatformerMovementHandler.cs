using UnityEngine;

public class NPCPlatformerMovementHandler : PlatformerMovementHandler {
    
    [SerializeField] protected float resistStateTime = 2f;

    private float resistStateTimer = 0f;

    protected override void FixedUpdate() {
        base.FixedUpdate();
        
        if (resistStateTimer < resistStateTime) {
            resistStateTimer += Time.fixedDeltaTime;
        }
    }

    // Stuff for handling the force resistance state.
    public void ResistMovementStart() {
        moveControlState = MoveControlState.Stationary;
        resistStateTimer = 0f;
    } 

    public void ResistMovementUpdate() {
        ResistMovement();
    }

    public bool ResistMovementShouldEnd() {
        return resistStateTimer >= resistStateTime;
    }

    // Stuff for the Stun State
    public void StunMovementStart() {
        moveControlState = MoveControlState.Stationary;

    }

    public void StunMovementUpdate() {
        moveControlState = MoveControlState.Stationary;

    }
}
