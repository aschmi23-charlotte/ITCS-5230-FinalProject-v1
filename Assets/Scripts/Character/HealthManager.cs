using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;

    [System.Serializable]
    public class HealthEvents {
        
        [SerializeField] public UnityEvent onTakeDamage;
        [SerializeField] public UnityEvent onNotDeadYet;
        [SerializeField] public UnityEvent onDeath;
    }

    [SerializeField] protected HealthEvents healthEvents;


    // Sets health without triggering any events. Use with caution.
    public float CurrentHealth {
        get {
            return currentHealth;
        }
        set {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        }
    }

    void Awake() {}

    public void RecieveDamage(float amount) {
        if (amount <= 0f) {
            Debug.LogFormat("Damage value {0} is less than or equal to zero. No effect.", amount);
            return;
        }

        CurrentHealth -= amount;
        healthEvents.onTakeDamage.Invoke();

        if (CurrentHealth == 0) {
            healthEvents.onDeath.Invoke();
        } else {
            healthEvents.onNotDeadYet.Invoke();
        }
    }
}
