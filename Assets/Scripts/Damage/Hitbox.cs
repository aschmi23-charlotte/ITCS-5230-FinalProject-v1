using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    [SerializeField] public float baseDamage = 10f;
    [SerializeField] public float baseStun = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created.
    [SerializeField] public ContactFilter2D filter;

    public Collider2D Col { get; private set; }
    void Awake() {
        Col = GetComponent<Collider2D>();
    }

    void FixedUpdate() {
        List<Collider2D> contacts = new List<Collider2D>();
        Col.Overlap(filter, contacts);

        foreach(Collider2D hit in contacts) {
            Hurtbox hurt = hit.GetComponent<Hurtbox>();
            if (hurt != null) {
                hurt.RecieveComboHit(baseDamage, baseStun);
            }
        }
    }
}
