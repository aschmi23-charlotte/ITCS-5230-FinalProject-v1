using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour {
    [SerializeField] protected HealthManager healthManager;
    [SerializeField] protected StunManager stunManager;
    [SerializeField] public UnityEvent onDamageHit;
    [SerializeField] public UnityEvent onStunHit;

    protected Collider2D col;

    void Awake() {
        col = GetComponent<Collider2D>();
    }

    public void RecieveDamageHit(float damageAmount) {
        onDamageHit.Invoke();

        healthManager.RecieveDamage(damageAmount);
    }

        public void RecieveStunHit(float stunAmount) {
        onStunHit.Invoke();

        stunManager.RecieveStun(stunAmount);
    }

}
