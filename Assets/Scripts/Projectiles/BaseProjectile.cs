using UnityEngine;
using UnityEngine.Events;

public abstract class BaseProjectile : MonoBehaviour {
	[Header("Projectile Behaviour")]
	[field: SerializeField] public Vector2 Direction { get; set; } = Vector2.right;
	[field: SerializeField] public float Speed { get; set; } = 20f;
	[field: SerializeField] public LayerMask HitMask { get; set; }

	public Vector2 InheritedVelocity { get; set; } = Vector2.zero;
	public UnityEvent onHit;

}
