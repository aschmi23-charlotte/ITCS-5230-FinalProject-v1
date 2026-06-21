using UnityEngine;

public abstract class MovementHandlerBase : MonoBehaviour {
    [Header("Movement Values")]
    // Target move speed for the character.
    [SerializeField] protected float moveSpeed = 15.0f;
    // The character's acceleration
    [SerializeField] protected float acceleration = 12.0f;
    [SerializeField] protected float resistStateAcceleration = 10.0f;
    
    [System.Serializable]
    public enum FacingDirection {
        Left,
        Right,
    }
    [Header("Facing Rendering")]

    [field: SerializeField] public SpriteRenderer MainSpriteRenderer {get; private set; }
    public FacingDirection spriteFacingDirection = FacingDirection.Right;
    public FacingDirection currentFacingDirection = FacingDirection.Right;

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

    protected virtual void Awake() {
        Body = GetComponent<Rigidbody2D>();
	    Col = GetComponent<Collider2D>();
    }

    protected virtual void Start() {}
    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}

    public void UpdateFacing(Vector2 direction) {
        if (direction.x > 0f) {
            currentFacingDirection = FacingDirection.Right;
        } else if (direction.x < 0f) {
            currentFacingDirection = FacingDirection.Left;
        }

        if (MainSpriteRenderer != null) {
            MainSpriteRenderer.flipX = spriteFacingDirection != currentFacingDirection;
        }

    }

    public Vector2 GetFacingAsVector() {
        switch (currentFacingDirection) {
            case FacingDirection.Left:
                return Vector2.left;
            case FacingDirection.Right:
            default:
                return Vector2.right;
        }
    }

    public abstract void ProcessMovementDirection(Vector2 direction);
    public abstract void ResistMovement();
}