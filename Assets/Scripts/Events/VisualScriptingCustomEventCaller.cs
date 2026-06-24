using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class VisualScriptingCustomEventCaller : MonoBehaviour {
    
    public List<CustomEventCall> customEventCalls;

    public Variables variables { get; private set; }

    void Awake() {
        variables = GetComponent<Variables>();
    }

    // Be advised that calling by label uses a linear search.
    // May not perform well with lots of CustomEventCalls defined.
    public void TriggerByLabel(string label) {
        foreach (CustomEventCall cec in customEventCalls) {
            if (cec.callLabel == label) {
                cec.Invoke(variables);
                break;
            }
        }

    }

    public void TriggerByIndex(int index) {
        customEventCalls[index].Invoke(variables);
    }
}
