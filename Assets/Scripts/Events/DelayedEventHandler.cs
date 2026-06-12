using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// A simple helper script that invoked UnityEvents after a delay
public class DelayedEventHandler : MonoBehaviour {
    [System.Serializable]
    protected class DelayedEvent {
        public float delayTime = 0f;
        public UnityEvent unityEvent;

        public IEnumerator InvokeCoroutine() {
            yield return new WaitForSeconds(delayTime);
            unityEvent.Invoke();
        }
    }

    [SerializeField] protected bool invokeImmediate = false;
    [SerializeField] protected List<DelayedEvent> events;

    public void InvokeAll() {
        for (int i = 0; i < events.Count; i++) {
            InvokeByIndex(i);
        }
    }
    public void InvokeByIndex(int index) {
        StartCoroutine(events[index].InvokeCoroutine());
    }

}
