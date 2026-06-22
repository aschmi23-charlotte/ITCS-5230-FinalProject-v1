using UnityEngine;

public class MovementAgentBrain : MonoBehaviour {
    public MovementHandlerBase Movement {get; private set; }

    void Awake() {
        Movement = GetComponent<MovementHandlerBase>();
    }

    public void UpdateMovement(Vector2 destination, float speedPercent) {
        Vector2 delta = destination - (Vector2)transform.position;
        Vector2 direction = delta.normalized;

        Movement.UpdateFacing(direction);
        Movement.ProcessMovementDirection(direction * speedPercent);
        
    }
}