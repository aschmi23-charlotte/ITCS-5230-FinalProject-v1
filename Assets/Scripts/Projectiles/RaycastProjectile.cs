using UnityEngine;
using UnityEngine.Events;

public class RaycastProjectile : BaseProjectile {


	protected Vector2 nextSafePos;
    void Update() {
		
    }

    void FixedUpdate() {
        Vector2 travel = (InheritedVelocity + Direction * Speed) * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, travel.normalized, travel.magnitude, HitMask);
        if ((bool)hit) {
            transform.position = hit.point;
            onHit.Invoke();
        } else {
        	nextSafePos = travel;
            transform.position += (Vector3)(travel);
        }

    }

}
