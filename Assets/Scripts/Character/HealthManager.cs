using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour {
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;

    [SerializeField] protected UnityEvent onTakeDamage;
    [SerializeField] protected UnityEvent onNotDeadYet;
    [SerializeField] protected UnityEvent onDeath;


    public float CurrentHealth {
        get {
            return currentHealth;
        }
        set {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        }
    }

    void Awake() {}

    public void TakeDamage(float amount) {
        CurrentHealth -= amount;

        onTakeDamage.Invoke();

        if (CurrentHealth == 0) {
            onDeath.Invoke();
        } else {
            onNotDeadYet.Invoke();
        }
    }

}
