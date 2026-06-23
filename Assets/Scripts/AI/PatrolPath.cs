using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolPath : MonoBehaviour {
    
    [System.Serializable]
    public enum FollowMode {
        Loop,
        OneWay,
        PingPong
    }

    [field: SerializeField] public FollowMode followMode { get; private set; } = FollowMode.OneWay;
    [field: SerializeField] public List<PatrolPathNode> nodes { get; private set; } = new List<PatrolPathNode>();

    // Informing each node of it's index on startup.
    void Start() {
        AssignIndices();
    }

    public void AssignIndices() {
        for (int i = 0; i < nodes.Count; i++) {
            nodes[i].Index = i;
        }
    }

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] protected bool alwaysDrawGizmos = false;
    [SerializeField] protected Color gizmoColor = Color.yellow;

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

        for (int i = 0; i < nodes.Count; i++) {
            PatrolPathNode node = nodes[i];
            PatrolPathNode nextNode = null;

            if (i < nodes.Count - 1) {
                // This is not the last node in the list.
                nextNode = nodes[i + 1];
            } else if (followMode == FollowMode.Loop) {
                nextNode = nodes[0];
            }

            if (nextNode != null) {
                Gizmos.DrawLine(node.transform.position, nextNode.transform.position);            
            }

            UnityEditor.Handles.Label(node.transform.position + Vector3.up * 0.2f, "Node " + i);
            
        }
    }

    // Quick Editor Button to create new nodes.
    [Button]
    public void AddNode() {
        GameObject newNode = new GameObject("PatrolPathNode " + nodes.Count);
        if (nodes.Count > 0) {
            newNode.transform.position = nodes[nodes.Count - 1].transform.position;
        } else {
            newNode.transform.position = transform.position;
        }

        nodes.Add(newNode.AddComponent<PatrolPathNode>());
        newNode.transform.parent = transform;
    }
#endif
} 