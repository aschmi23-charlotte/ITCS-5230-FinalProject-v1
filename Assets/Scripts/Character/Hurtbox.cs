using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour {
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

    void Awake() {
        col = GetComponent<Collider2D>();
    }

    // If an action is supposed to do both damage and stun, then use this method instead.
    // that way, we don't invoke onHit twice.
    public void RecieveComboHit(float damageAmount, float stunAmount) {
        hurtEvents.onHurt.Invoke();
        hurtEvents.onDamageHurt.Invoke();
        hurtEvents.onStunHurt.Invoke();

        if (healthManager != null) {
            healthManager.RecieveDamage(damageAmount);   
        }

        if (stunManager != null) {
            stunManager.RecieveStunPointDamage(stunAmount);  
        }
    }

    public void RecieveDamageHit(float damageAmount) {
        hurtEvents.onHurt.Invoke();
        hurtEvents.onDamageHurt.Invoke();

        if (healthManager != null) {
            healthManager.RecieveDamage(damageAmount);   
        }
    }

    public void RecieveStunHit(float stunAmount) {
        hurtEvents.onHurt.Invoke();
        hurtEvents.onStunHurt.Invoke();

        if (stunManager != null) {
            stunManager.RecieveStunPointDamage(stunAmount);  
        }
    }

}
