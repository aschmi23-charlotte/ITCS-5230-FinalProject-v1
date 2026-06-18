using UnityEngine;
using UnityEngine.Events;

public abstract class ItemPickupBase : MonoBehaviour {
    // Editor Fields:
    [Header("General Pickup Info")]
    [SerializeField] protected bool destroyOnPickup = true;
    [SerializeField] protected UnityEvent onPickupEvent;
    
    // Attributes:
    protected Collider2D Trigger {get; private set; }

    void Awake() {
        Trigger = GetComponent<Collider2D>();
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Triggered");
        PlayerBrain player = other.GetComponent<PlayerBrain>();
        // If this is not the player, ignore it.
        if (player == null) { return; }
        
        // Handle Pickup:
        OnItemPickup(player);
        onPickupEvent.Invoke();
        if (destroyOnPickup) {
            Destroy(gameObject);
        }
    }

    public abstract void OnItemPickup(PlayerBrain player);

}