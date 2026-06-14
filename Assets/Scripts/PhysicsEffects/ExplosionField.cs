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
    [field: SerializeField] public ForceMode2D ForceMode { get; set; } = ForceMode2D.Impulse;
    [field: SerializeField] public AnimationCurve ForceCurve { get; set; }
    [field: SerializeField] public bool Continuous { get; set; } = false;
    [field: SerializeField] LayerMask HitMask { get; set; }
    [field: SerializeField] public bool destroyOnDetonate { get; set; } = true;
    [SerializeField] public UnityEvent onDetonate;
    [SerializeField] public UnityEvent onUpdateContinuous;
    [SerializeField] public UnityEvent onDeactivateContinuous;


    public bool RunningContinuous { get; private set; } = false;

    void FixedUpdate() {
        if (immediate) {
            immediate = false;
            Detonate();
        } else if (RunningContinuous) {
            ApplyExplosionForce();
            onUpdateContinuous.Invoke();
        }
    }

    public void Detonate() {
        if (Continuous) {
            RunningContinuous = true;
        } else {
            ApplyExplosionForce();
        }

        onDetonate.Invoke();


        if (destroyOnDetonate) {
            Destroy(gameObject);
        }
    }

    public void DeactivateContinuous() {
        RunningContinuous = false;
        onDeactivateContinuous.Invoke();
    }

    void ApplyExplosionForce() {
        ExplosionForce2D.PerformCurvedExplosionForce((Vector2)transform.position + Offset, BlastRadius, BlastForce, HitMask, ForceCurve, ForceMode);
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        // Draw blast radius sphere
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawSphere((Vector2)transform.position + Offset, BlastRadius);
    }
#endif
}

