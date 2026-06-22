using UnityEngine;

public class HostileTag : MonoBehaviour {
    [System.Serializable]
    [System.Flags]
    public enum HostileCategories {
        None = 0,
        Player = 1,
        Enemy = 2
    }

    public HostileCategories hostileCategories;
    public Vector2 detectionPosition;

    public Vector2 GetWorldDetectionPosition() {
        return (Vector2)transform.position + detectionPosition;
    }

    
#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] protected bool alwaysDrawGizmos = false;
    [SerializeField] protected Color gizmoColor = Color.purple;
    
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
        Gizmos.DrawWireSphere((Vector3)GetWorldDetectionPosition(), 0.1f);
    }
#endif
}