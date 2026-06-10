using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GenericProjectile : MonoBehaviour {
    [System.Serializable]
    public enum MovementType {
        MovePosition,
        MaintainVelocity,
        SetStartingVelocity,
    }


    

    public Rigidbody2D Body { get; private set; }
    void Awake() {
        Body = GetComponent<Rigidbody2D>();
    }

    
    void Update() {
        
    }
}
