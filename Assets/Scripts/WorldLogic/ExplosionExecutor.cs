using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


public class ExplosionExecutor: MonoBehaviour {
    [Header("Behavior")]
    [SerializeField] bool immediate = false;

    [Header("Explosion Properties")]
    // I want these accessable in Visual Scripting, but still serialized.
    [field: SerializeField] public Vector2 Offset { get; set; } = Vector2.zero;
    [field: SerializeField] public float BlastForce { get; set; } = 20f;
    [field: SerializeField] public float BlastRadius { get; set; } = 5f;
    [field: SerializeField] LayerMask HitMask { get; set; }

    public UnityEvent onDetonate; 

    void FixedUpdate() {
        if (immediate) {
            immediate = false;
            Detonate();
        }
    }

    public void Detonate() {
        ExplosionForce2D.PerformExplosion((Vector2)transform.position + Offset, BlastRadius, BlastForce, HitMask);
        onDetonate.Invoke();
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, 1f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        //Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawSphere((Vector2)transform.position + Offset, BlastRadius);
    }
#endif
}

