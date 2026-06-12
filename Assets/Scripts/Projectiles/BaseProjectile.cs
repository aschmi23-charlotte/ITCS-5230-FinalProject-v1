using UnityEngine;
using UnityEngine.Events;

public abstract class BaseProjectile : MonoBehaviour {
	[Header("Projectile Behaviour")]
	// I want these accessable in Visual Scripting, but still serialized.
	[field: SerializeField] public Vector2 Direction { get; set; } = Vector2.right;
    [field: SerializeField] public float Speed { get; set; } = 20f;
    [field: SerializeField] public LayerMask HitMask { get; set; }
    [field: SerializeField] public bool DestroyOnHit { get; set; } = true;

	[SerializeField] public UnityEvent onHitEvent;

    public Vector2 InheritedVelocity { get; set; } = Vector2.zero;
    public virtual void OnHit() {
		onHitEvent.Invoke();
		if (DestroyOnHit) {
			Destroy(gameObject);
		}

    }
}
