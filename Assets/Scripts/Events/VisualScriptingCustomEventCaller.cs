using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(Variables))]
public class VisualScriptingCustomEventCaller : MonoBehaviour {
    [System.Serializable]
    public class CustomEventCall {
        public GameObject target;
        public string eventName = "CustomEventName";        
        public string[] argumentVariableNames;

        public void Invoke(Variables vars) {
            object[] args = new object[argumentVariableNames.Length];

            for (int i = 0; i < args.Length; i++) {
                args[i] = Variables.Object(vars).Get(argumentVariableNames[i]);
            }

            EventBus.Trigger("Custom", target, new CustomEventArgs(eventName, args));
        }
    }
    public List<CustomEventCall> customEventCalls;

    public Variables variables { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        variables = GetComponent<Variables>();
    }

    public void TriggerByIndex(int index) {
        customEventCalls[index].Invoke(variables);
    }
}
