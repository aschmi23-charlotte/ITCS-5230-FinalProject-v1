using UnityEngine;

public class MovementAgentBrain : MonoBehaviour {
    // Editor fields
    [SerializeField] protected Vector2 destination;
    [System.Serializable]
    public enum AgentDesire {
        Stop,
        Move    
    }
    // Set what the agent is TRYING to do.
    [SerializeField] protected AgentDesire desire = AgentDesire.Stop;

    // Attributes
    // Might need to trigger something when this changes. We'll see.
    public AgentDesire Desire {

        get {
            return desire;
        }
        set {
            desire = value;
        }
    }
    public Vector2 Destination {
        get {
            return destination;
        }
        set {
            destination = value;
        }
    }
    public MovementHandlerBase Movement {get; private set; }

    void Awake() {
        Movement = GetComponent<MovementHandlerBase>();
    }

    void FixedUpdate() {
        UpdateMovement();
    }

    public void UpdateMovement() {
        Vector2 delta = Destination - (Vector2)transform.position;
        Vector2 direction = delta.normalized;

        Movement.ProcessMovementDirection(direction);
        
    }
}