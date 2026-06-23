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
    [field: SerializeField] public float BlastRadius { get; set; } = 5f;
    [field: SerializeField] public float BlastForce { get; set; } = 20f;
    [field: SerializeField] public ForceMode2D ForceMode { get; set; } = ForceMode2D.Impulse;
    [field: SerializeField] LayerMask ForceHitMask { get; set; }
    [field: SerializeField] public AnimationCurve ForceCurve { get; set; }
    [field: SerializeField] public float BlastDamage { get; set; } = 20f;
    [field: SerializeField] public LayerMask DamageHitMask { get; set; }
    [field: SerializeField] public AnimationCurve DamageCurve { get; set; }
    [field: SerializeField] public bool Continuous { get; set; } = false;
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
            ApplyExplosion();
            onUpdateContinuous.Invoke();
        }
    }

    public void Detonate() {
        if (Continuous) {
            RunningContinuous = true;
        } else {
            ApplyExplosion();
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

    void ApplyExplosion() {
        // No point in running extra code that will have no effect. 
        if (BlastForce != 0f) {
            ExplosionForce2D.PerformCurvedExplosionForce((Vector2)transform.position + Offset, BlastRadius, BlastForce, ForceHitMask, ForceCurve, ForceMode);    
        }
        
        if (BlastDamage > 0f) {
            // If continuous, Make it damage per seceond.
            ExplosionForce2D.PerformCurvedExplosionDamage((Vector2)transform.position + Offset, BlastRadius, Continuous ? BlastDamage * Time.fixedDeltaTime :  BlastDamage, DamageHitMask, DamageCurve);    
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {
        // Draw blast radius sphere
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawSphere((Vector2)transform.position + Offset, BlastRadius);
    }
#endif
}

