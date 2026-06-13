using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour {
    [SerializeField] protected HealthManager healthManager;

    protected Collider2D col;

    void Awake() {
        col = GetComponent<Collider2D>();
    }

}
