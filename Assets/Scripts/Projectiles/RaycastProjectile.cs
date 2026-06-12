using UnityEngine;
using UnityEngine.Events;

public class RaycastProjectile : BaseProjectile {

    void FixedUpdate() {
        Vector2 travel = (InheritedVelocity + Direction * Speed) * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, travel.normalized, travel.magnitude, HitMask);
        if ((bool)hit) {
            transform.position = hit.point;
            OnHit();
        } else {
            transform.position += (Vector3)(travel);
        }
    }

}
