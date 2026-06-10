using System.Diagnostics;
using UnityEngine;

public class ImploderExploderAssembly : ProjectileWeaponBase {

    // Fortunately, the IEA state management should be a LOT simpler than the Kinetic Laser.
    [Header("ImploderExploderAssembly")]
    [SerializeField] GameObject projectilePrefab;

    void FixedUpdate() {
        if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
            RaycastProjectile newProjectile = Instantiate(projectilePrefab).GetComponent<RaycastProjectile>();
            newProjectile.transform.position = firePoint.position;
            newProjectile.Direction = ParentWeaponSystem.AimDirection;
            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
            if (rb != null) {
                newProjectile.InheritedVelocity = rb.linearVelocity;
            }
        }
    }
}
