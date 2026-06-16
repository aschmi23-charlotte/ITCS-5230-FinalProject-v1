using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class StunManager : MonoBehaviour {
    [SerializeField] protected float maxStunPoints = 100f;
    [SerializeField] protected float currentStunPoints = 100f;
    [SerializeField] protected float stunPointsRecoveryPerSecond = 10f;
    [SerializeField] protected float stunPointRecoveryDelay = 3f;
    [SerializeField] protected float stunTime = 8f;
    [SerializeField] protected bool restoreStunPointsOnStunEnd = true;

    [System.Serializable]
    public class StunEvents {
        [SerializeField] public UnityEvent onStunStart;
        [SerializeField] public UnityEvent onStunEnd;
    }

    [SerializeField] protected StunEvents stunEvents;

    protected float recoveryDelayTimer = 0f;
    protected float stunTimer = 0f;

    // Sets stun without triggering any events. Use with caution.
    public float CurrentStunPoints {
        get {
            return currentStunPoints;
        }
        set {
            currentStunPoints = Mathf.Clamp(value, 0f, maxStunPoints);
        }
    }

    void FixedUpdate() {
        // Update the Stun State
        if (IsStunned()) {
            stunTimer += Time.fixedDeltaTime; 
            if (!IsStunned()) {
                // Stun ended.
                StunEnd();
            }
        }

        else if (recoveryDelayTimer < stunPointRecoveryDelay) {
            recoveryDelayTimer += Time.fixedDeltaTime;
        }
        
        else {
            CurrentStunPoints += stunPointsRecoveryPerSecond * Time.fixedDeltaTime;
        }
    }

    public bool IsStunned() {
        return stunTimer < stunTime;
    }

    public void RecieveStunPointDamage(float amount) {
        if (amount <= 0f) {
            Debug.LogFormat("Stun value {0} is less than or equal to zero. No effect.", amount);
            return;
        }

        CurrentStunPoints -= amount;
        recoveryDelayTimer = 0f;

        if (CurrentStunPoints == 0) {
            StunStart();
        }
    }

    public void StunStart() {
        stunTimer = 0f;
        stunEvents.onStunStart.Invoke();
    }

    public void StunEnd() {
        recoveryDelayTimer = stunPointRecoveryDelay;
        if (restoreStunPointsOnStunEnd) {
            CurrentStunPoints = maxStunPoints;
        }
        stunEvents.onStunEnd.Invoke();
    }
}
