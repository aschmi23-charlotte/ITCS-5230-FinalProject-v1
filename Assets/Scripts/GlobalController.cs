using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalController : MonoBehaviour {

    public static GlobalController Instance { get; private set; }
    void Awake() {
        Debug.Assert(Instance == null, "Multiple instances of GlobalController present!");
        if (Instance == null) {
            Instance = this;
            Debug.Log("GlobalController instance awakened.");
        }
    }

    void Start() {
        
    }

    
    void Update() {
        
    }
}
