using UnityEngine;

public class Hitbox : MonoBehaviour {

    [SerializeField] public float baseDamage = 10f;
    [SerializeField] public float baseStun = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created.
    
    void OnTriggerStay2D(Collider2D collider) {
        // Apparently, we have check this manually. Huh.
        if (!enabled) return;
        
        // We're relying on Hurtbox invulnFrames to prevent constant damage ticks.
        // Doesn't support damage over time right now.
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (hurtbox == null) return;
        
        hurtbox.RecieveComboHit(baseDamage, baseStun);
    }
}
