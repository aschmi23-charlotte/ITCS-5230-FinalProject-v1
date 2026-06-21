using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour {
    [SerializeField] public float invulnFrameTime = 2f;

    [SerializeField] protected HealthManager healthManager;
    [SerializeField] protected StunManager stunManager;

    [System.Serializable]
    public class HurtEvents {
        [SerializeField] public UnityEvent onHurt;
        [SerializeField] public UnityEvent onDamageHurt;
        [SerializeField] public UnityEvent onStunHurt;
    }

    [SerializeField] protected HurtEvents hurtEvents;

    protected Collider2D col;
    protected float invulnFrameTimer = 0f;

    void Awake() {
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate() {
        if (IsInvulnerable()) {
            invulnFrameTimer += Time.fixedDeltaTime;
        }
    }

    public void TriggerInvulnFrames() {
        invulnFrameTimer = 0f;
    }

    public bool IsInvulnerable() {
        return invulnFrameTimer < invulnFrameTime;
    }

    // If an action is supposed to do both damage and stun, then use this method instead.
    // that way, we don't invoke onHit twice.
    public void RecieveComboHit(float damageAmount, float stunAmount, bool shouldTriggerInvuln = true) {
        // Take no damage while invulnerable
        if (IsInvulnerable()) {
            return;
        }

        hurtEvents.onHurt.Invoke();
        hurtEvents.onDamageHurt.Invoke();
        hurtEvents.onStunHurt.Invoke();

        if (healthManager != null) {
            healthManager.RecieveDamage(damageAmount);   
        }

        if (stunManager != null) {
            stunManager.RecieveStunPointDamage(stunAmount);  
        }

        if (shouldTriggerInvuln) {
            TriggerInvulnFrames();
        }
    }

    public void RecieveDamageHit(float damageAmount, bool shouldTriggerInvuln = true) {
        // Take no damage while invulnerable
        if (IsInvulnerable()) {
            return;
        }

        hurtEvents.onHurt.Invoke();
        hurtEvents.onDamageHurt.Invoke();

        if (healthManager != null) {
            healthManager.RecieveDamage(damageAmount);   
        }

        if (shouldTriggerInvuln) {
            TriggerInvulnFrames();
        }
    }

    public void RecieveStunHit(float stunAmount, bool shouldTriggerInvuln = true) {
        // Take no damage while invulnerable
        if (IsInvulnerable()) {
            return;
        }

        hurtEvents.onHurt.Invoke();
        hurtEvents.onStunHurt.Invoke();

        if (stunManager != null) {
            stunManager.RecieveStunPointDamage(stunAmount);  
        }

        if (shouldTriggerInvuln) {
            TriggerInvulnFrames();
        }
    }

}
