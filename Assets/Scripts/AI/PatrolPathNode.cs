using UnityEngine;

public class PatrolPathNode : MonoBehaviour {
    public float waitTime = 0f;
    public float targetRadius = 1f;

    // Assigned by PatrolPath on Start:
    public int Index {get; set; }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] protected bool alwaysDrawGizmos = false;
    [SerializeField] protected Color gizmoColor = Color.yellow;
#endif

    public PatrolPath GetPatrolPath() {
        return GetComponentInParent<PatrolPath>();
    }

#if UNITY_EDITOR
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

        Gizmos.DrawWireSphere(transform.position, targetRadius);
    }
#endif
}