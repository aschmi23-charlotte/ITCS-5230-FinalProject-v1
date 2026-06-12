using UnityEngine;
using UnityEngine.Events;

public class SimpleTriggerSwitch : MonoBehaviour {

    [System.Serializable]
    public enum ActivationMode {
        ObjectCount,
        Mass
    }

    [SerializeField] protected ActivationMode activationMode = ActivationMode.ObjectCount;
    [SerializeField] protected int activationCount = 1;
    [SerializeField] protected float activationMass = 1f;

    [SerializeField] UnityEvent OnActivate;
    [SerializeField] UnityEvent WhileActive;
    [SerializeField] UnityEvent OnDeactivate;

    private int currentCount = 0;
    private float currentMass = 0;

    private bool CheckActive() {
        switch(activationMode) {
            case ActivationMode.ObjectCount:
                return currentCount >= activationCount;
            case ActivationMode.Mass:
            default:
                return currentMass >= activationMass;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        bool beforeState = CheckActive();

        if (activationMode == ActivationMode.ObjectCount) {
            currentCount++;
            if (!beforeState && CheckActive()) {
                OnActivate.Invoke();
            }

        } else if (activationMode == ActivationMode.Mass && collision.attachedRigidbody != null) {
            currentMass += collision.attachedRigidbody.mass;
            if (!beforeState && CheckActive()) {
                OnActivate.Invoke();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (CheckActive()) {
            WhileActive.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        bool beforeState = CheckActive();
        if (activationMode == ActivationMode.ObjectCount) {
            currentCount--;
            if (beforeState && !CheckActive()) {
                OnDeactivate.Invoke();
            }
        } else if (activationMode == ActivationMode.Mass && collision.attachedRigidbody != null) {
            currentMass -= collision.attachedRigidbody.mass;
            if (beforeState && !CheckActive()) {
                OnDeactivate.Invoke();
            }
        }
    }
}
