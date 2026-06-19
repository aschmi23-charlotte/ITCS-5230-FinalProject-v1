using UnityEngine;

public class NPCFlyingMovementHandler : FlyingMovementHandler {
    
    public Vector2 moveInput;

    protected override void FixedUpdate() {
        ProcessMovementDirection(moveInput);
    }

}