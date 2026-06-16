using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[System.Serializable]
public class CustomEventCall {
    [SerializeField] public string callLabel = "CustomEvent Call";
    [SerializeField] public string eventName = "CustomEventName";
    [SerializeField] public GameObject target;

    [System.Serializable]
    public class Argument {
        [System.Serializable]
        public enum ArgumentType {
            Null,
            Bool,
            Int,
            Float,
            String,
            Vector2,
            Vector3,
            GameObject,
            Component,
            Object,
            VSVariable,
        }

        [SerializeField] public ArgumentType argumentType = ArgumentType.Null;
        [SerializeField] public bool boolArg;
        [SerializeField] public int intArg;
        [SerializeField] public float floatArg;
        [SerializeField] public string stringArg;
        [SerializeField] public Vector2 vector2Arg;
        [SerializeField] public Vector3 vector3Arg;
        [SerializeField] public GameObject gameObjectArg;
        [SerializeField] public Component gameObjectComponentArg;
        [SerializeField] public Object unityEngineObjectArg;
        [SerializeField] public string visualScriptingVariableName;


        public object GetArgObject() {
            return GetArgObject(null);
        }

        public object GetArgObject(Variables vars) {
            switch (argumentType) {
                case ArgumentType.Null:
                default:
                    return null;
                case ArgumentType.Bool:
                    return boolArg;
                case ArgumentType.Int:
                    return intArg;
                case ArgumentType.Float:
                    return floatArg;
                case ArgumentType.String:
                    return stringArg;
                case ArgumentType.Vector2:
                    return vector2Arg;
                case ArgumentType.Vector3:
                    return vector3Arg;
                case ArgumentType.GameObject:
                    return gameObjectArg;
                case ArgumentType.Component:
                    return gameObjectComponentArg;
                case ArgumentType.Object:
                    return unityEngineObjectArg;
                case ArgumentType.VSVariable:
                    return vars.declarations.Get(visualScriptingVariableName);
            }
        }
    }

    public List<Argument> arguments;

    public void Invoke() {
        Invoke(null);
    }

    public void Invoke(Variables vars) {
        // We want to pass args as an array, not param. Hence, CustomEvent.Trigger won't work.
        object[] args = new object[arguments.Count];

        for (int i = 0; i < args.Length; i++) {
            args[i] = arguments[i].GetArgObject(vars);
        }

        EventBus.Trigger("Custom", target, new CustomEventArgs(eventName, args));
    }
}