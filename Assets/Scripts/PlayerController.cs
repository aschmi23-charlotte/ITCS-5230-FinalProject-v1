using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour {
    [Header("Stats")]
    public float moveSpeed = 5.0f;

    // Components
    public SpriteRenderer render { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D col { get; private set; }

    void Start() {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        
    }
}
