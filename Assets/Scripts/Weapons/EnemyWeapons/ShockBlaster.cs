using JetBrains.Annotations;
using UnityEngine;

public class ShockBlaster : WeaponBase {
    
    // Assume this is attacked to the mainSpriteRenderer's gameObject.
    public Animator WeaponAnimator { get; private set; }

    // Assume attached to this gameObject
    public Hitbox Hit {get; private set; }
    public BoxCollider2D Trigger {get; private set;}

    protected override void Awake() {
        base.Awake();

        WeaponAnimator = mainSpriteRenderer.GetComponent<Animator>();
        Hit = GetComponent<Hitbox>();
        Trigger = GetComponent<BoxCollider2D>();
    }

    public void UpdateHitbox(bool active) {
        Hit.enabled = active;
    }
}