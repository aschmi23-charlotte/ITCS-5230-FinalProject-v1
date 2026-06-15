using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;

    [SerializeField] protected UnityEvent onTakeDamage;
    [SerializeField] protected UnityEvent onNotDeadYet;
    [SerializeField] protected UnityEvent onDeath;

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
        onTakeDamage.Invoke();

        if (CurrentHealth == 0) {
            onDeath.Invoke();
        } else {
            onNotDeadYet.Invoke();
        }
    }
}
