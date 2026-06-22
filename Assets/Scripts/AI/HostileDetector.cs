using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HostileDetector : MonoBehaviour {
    [System.Serializable]
    public class DetectionArea {
        public enum DetectionShape {
            Circle,
            Arc
        }

        public DetectionShape shape = DetectionShape.Circle;

        public Vector2 offset = Vector2.zero;
        public float radius = 10f;
        public float arc = 90f;
        public float angle = 0f;

        public Vector2 GetArcUpperEndpoint() {
            return VectorMathHelpers.RotateAround(offset + Vector2.right * radius, offset, angle + arc/2);
        }

        public Vector2 GetArcLowerEndpoint() {
            return VectorMathHelpers.RotateAround(offset + Vector2.right * radius, offset, angle - arc/2);
        }

        public bool CheckIfInRange(HostileDetector host, HostileTag target) {
            switch (shape) {
                case DetectionShape.Circle:
                default:
                    return (target.transform.position - host.transform.position).magnitude <= radius;
                case DetectionShape.Arc:
                    // To far away. That's the easier calculation, so do that first.
                    if ((target.transform.position - host.transform.position).magnitude > radius) {
                        return false;    
                    }

                    float checkAngle = VectorMathHelpers.GetAngle(target.transform.position, host.transform.position);
                    return angle - arc/2 < checkAngle && checkAngle < angle + arc/2;
            }

        }
    }

    [SerializeField] protected DetectionArea detectionArea;
    [SerializeField] protected HostileTag.HostileCategories detectCategories;
    
    // We don't necessarily need to check every FixedUpdate. That could be costly.
    [SerializeField] protected float delayBetweenChecks = 0.0f;
    [SerializeField] protected bool autoRunDetection = false;

    [System.Serializable]
    public class TargetEvents {
        public UnityEvent onCurrentTargetUpdated;
    }

    [SerializeField] protected TargetEvents targetEvents; 

    public bool AnyTargetsDetected {get; private set; } = false;
    public HostileTag CurrentTarget {get; private set; } = null;
    public Vector2 TargetLastKnownLocation {get; private set; }
    
    protected HostileTag selfTag = null;
    protected float checkTimer = 0f;

    void Awake() {
        // Might be null.
        selfTag = GetComponent<HostileTag>();
    }

    void FixedUpdate() {
        if (autoRunDetection) {
            RunDetectionTick();
        }
    }

    // To be called from the StateMachine when in an appropriate state.
    public void RunDetectionTick() {
        if (checkTimer < delayBetweenChecks) {
            checkTimer += Time.fixedDeltaTime;
        } else {
            checkTimer = 0f;
            DetectionCheckNow();
        }
    }

    public bool DetectionCheckNow() {
        HostileTag[] hostiles = FindObjectsByType<HostileTag>();

        AnyTargetsDetected = false;
        foreach (HostileTag hostile in hostiles) {
            // We are not suicidal.
            if (hostile == selfTag) {
                continue;
            }

            // Check if this hostile has a category we care about.
            if ((hostile.hostileCategories & detectCategories ) == HostileTag.HostileCategories.None) {
                continue;
            }

            if (detectionArea.CheckIfInRange(this, hostile)) {
                AnyTargetsDetected = true;
                InformOfTarget(hostile);
            }
        }

        return AnyTargetsDetected;
    }

    // Returns true if the detector wants to prioritize the new hostile.
    public bool InformOfTarget(HostileTag hostile) {
        if (CurrentTarget == null) {
            CurrentTarget = hostile;
        } else {
            // Prefer the closer enemy.
            CurrentTarget = 
                Vector3.Distance(hostile.transform.position, transform.position) < Vector3.Distance(CurrentTarget.transform.position, transform.position)
                ? hostile : CurrentTarget;
        }

        TargetLastKnownLocation = CurrentTarget.transform.position;
        
        if (CurrentTarget == hostile) {
            targetEvents.onCurrentTargetUpdated.Invoke();
            return true;
        } else {
            return false;
        }
    }

    public bool CanStillDetectTarget() {
        return CurrentTarget != null && detectionArea.CheckIfInRange(this, CurrentTarget);
    }

    public bool HasTarget() {
        return CurrentTarget != null;
    }

    public void ClearTarget() {
        CurrentTarget = null;
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] protected bool alwaysDrawGizmos = false;
    [SerializeField] protected Color gizmoColor = Color.red;
    
    void OnDrawGizmos() {
        if (alwaysDrawGizmos) {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected() {
        if (!alwaysDrawGizmos) {
            DrawGizmos();
        }
    }

    void DrawGizmos() {
        Gizmos.color = gizmoColor;

        switch (detectionArea.shape) {
            case DetectionArea.DetectionShape.Circle:
            default:
                Gizmos.DrawWireSphere(transform.position + (Vector3)detectionArea.offset, detectionArea.radius);
                break;
            case DetectionArea.DetectionShape.Arc:
                Gizmos.DrawLine(transform.position + (Vector3)detectionArea.offset, transform.position + (Vector3)detectionArea.GetArcLowerEndpoint());
                Gizmos.DrawLine(transform.position + (Vector3)detectionArea.offset, transform.position + (Vector3)detectionArea.GetArcUpperEndpoint());
                break;
        }
    }
#endif
}