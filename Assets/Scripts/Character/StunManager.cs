using UnityEngine;
using UnityEngine.Events;

public class StunManager : MonoBehaviour {
    [SerializeField] protected float maxStunPoints = 100f;
    [SerializeField] protected float currentStunPoints = 100f;
    [SerializeField] protected float stunPointsRecoveryPerSecond = 10f;

    [SerializeField] protected UnityEvent onStun;

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
        CurrentStunPoints += stunPointsRecoveryPerSecond * Time.fixedDeltaTime;
    }

    public void RecieveStun(float amount) {
        if (amount <= 0f) {
            Debug.LogFormat("Stun value {0} is less than or equal to zero. No effect.", amount);
            return;
        }

        CurrentStunPoints -= amount;

        if (CurrentStunPoints == 0) {
            onStun.Invoke();
        }
    }
}
