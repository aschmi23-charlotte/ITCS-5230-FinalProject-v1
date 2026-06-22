using UnityEngine;
using Unity.Behavior;
using Unity.VisualScripting;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class BehaviorVariableBridge : MonoBehaviour {
    protected BehaviorGraphAgent agent;

    void Awake() {
        agent = GetComponent<BehaviorGraphAgent>();
    }

    // Generic base functions:
    public T GetGenericByName<T>(string name) {
        agent.GetVariable<T>(name, out BlackboardVariable<T> variable);

        if (variable == null) {
            Debug.LogErrorFormat("No Blackboard Variable named {0}", name);
            return default;
        }

        return variable.Value;
    }

    public void SetGenericByName<T>(string name, T value) {
        agent.GetVariable<T>(name, out BlackboardVariable<T> variable);

        if (variable == null) {
            Debug.LogErrorFormat("No Blackboard Variable named {0}", name);
            return;
        }

        variable.Value = value;
    }

    // Specifics:
    public bool GetBoolByName(string name) => GetGenericByName<bool>(name);
    public void SetBoolByName(string name, bool value) => SetGenericByName<bool>(name, value);
    public int GetIntByName(string name) => GetGenericByName<int>(name);
    public void SetIntByName(string name, int value) => SetGenericByName<int>(name, value);
    public float GetFloatByName(string name) => GetGenericByName<float>(name);
    public void SetFloatByName(string name, float value) => SetGenericByName<float>(name, value);
    public string GetStringByName(string name) => GetGenericByName<string>(name);
    public void SetStringByName(string name, string value) => SetGenericByName<string>(name, value);
    public Vector2 GetVector2ByName(string name) => GetGenericByName<Vector2>(name);
    public void SetVector2ByName(string name, Vector2 value) => SetGenericByName<Vector2>(name, value);
    public Vector3 GetVector3ByName(string name) => GetGenericByName<Vector3>(name);
    public void SetVector3ByName(string name, Vector3 value) => SetGenericByName<Vector3>(name, value);
    public object GetObjectByName(string name) => GetGenericByName<object>(name);
    public void SetObjectByName(string name, object value) => SetGenericByName<object>(name, value);
    public Object GetUnityObjectByName(string name) => GetGenericByName<Object>(name);
    public void SetUnityObjectByName(string name, Object value) => SetGenericByName<Object>(name, value);
    public Component GetComponentByName(string name) => GetGenericByName<Component>(name);
    public void SetComponentByName(string name, Component value) => SetGenericByName<Component>(name, value);
    public GameObject GetGameObjectByName(string name) => GetGenericByName<GameObject>(name);
    public void SetGameObjectByName(string name, GameObject value) => SetGenericByName<GameObject>(name, value);
    public ScriptableObject GetScriptableObjectByName(string name) => GetGenericByName<ScriptableObject>(name);
    public void SetScriptableObjectByName(string name, ScriptableObject value) => SetGenericByName<ScriptableObject>(name, value);

}