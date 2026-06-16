using UnityEngine;

public class EnemyPlatformerMovementHandler : PlatformerMovementHandler {
    
    [SerializeField] protected float resistStateTime = 2f;

    private float resistStateTimer = 0f;

    public void FixedUpdate() {
        if (resistStateTimer < resistStateTime) {
            resistStateTimer += Time.fixedDeltaTime;
        }
    }

    // Stuff for handling the force resistance state.
    public void State_ResistStart() {
        moveControlState = MoveControlState.Stationary;
        resistStateTimer = 0f;
    } 

    public void State_ResistUpdate() {
        ResistMovement();
    }

    public bool State_ResistShouldEnd() {
        return resistStateTimer >= resistStateTime;
    }

    // Stuff for the Stun State
    public void State_StunStart() {
        moveControlState = MoveControlState.Stationary;

    }

    public void State_StunUpdate() {
        moveControlState = MoveControlState.Stationary;

    }
}
