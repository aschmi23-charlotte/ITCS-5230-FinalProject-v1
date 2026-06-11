using System.Diagnostics;
using UnityEngine;

public class ImploderExploderAssembly : ProjectileWeaponBase {

    [System.Serializable]
    protected class WeaponColors {
        public Gradient cooldownColorGradient;
    }

    // Fortunately, the IEA state management should be a LOT simpler than the Kinetic Laser.
    [Header("IEA Visuals")]
    [SerializeField] WeaponColors weaponColors;
    [SerializeField] GameObject primaryProjectilePrefab;
    [SerializeField] GameObject secondaryProjectilePrefab;
    [SerializeField] float fireCooldown = 0.6f;

    private float fireCooldownTimer = 0f;

    // This one is a LOT simpler than the Kinetic Laser.
    // Also, a charge attack on this thing may be... too strong.
    void FixedUpdate() {
        SetCooldownVisual();
        if (fireCooldownTimer > 0f) {
            fireCooldownTimer -= Time.fixedDeltaTime;
            return;
        }

        if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
            FireProjectile(primaryProjectilePrefab);

        } else if (secondaryInputStatus == WeaponSystem.InputStatus.Pressed) {

            FireProjectile(secondaryProjectilePrefab);
        }
    }

    private void FireProjectile(GameObject projectilePrefab) {
        RaycastProjectile newProjectile = Instantiate(projectilePrefab).GetComponent<RaycastProjectile>();
        newProjectile.transform.position = firePoint.position;
        newProjectile.Direction = ParentWeaponSystem.AimDirection;
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null) {
            newProjectile.InheritedVelocity = rb.linearVelocity;
        }
        fireCooldownTimer = fireCooldown;
    }

    protected void SetCooldownVisual() {
        mainSpriteRenderer.color = weaponColors.cooldownColorGradient.Evaluate(fireCooldownTimer / fireCooldown);
    }
}
