using System.Diagnostics;
using UnityEngine;

public class ImploderExploderAssembly : ProjectileWeaponBase {

    [System.Serializable]
    protected class WeaponColors {
        public Gradient cooldownColorGradient;
    }

    [Header("Ammo Consumtion")]
    [SerializeField] int primaryAmmoCost = 1;
    [SerializeField] int secondaryAmmoCost = 3;

    // Fortunately, the IEA state management should be a LOT simpler than the Kinetic Laser.
    [Header("IEA Visuals")]
    [SerializeField] WeaponColors weaponColors;
    [SerializeField] GameObject primaryProjectilePrefab;
    [SerializeField] GameObject secondaryProjectilePrefab;
    [SerializeField] float primaryFireCooldown = 0.6f;
    [SerializeField] float secondaryFireCooldown = 5f;

    private float primaryFireCooldownTimer = 0f;
    private float secondaryireCooldownTimer = 0f;

    // This one is a LOT simpler than the Kinetic Laser.
    // Also, a charge attack on this thing may be... too strong.
    protected override void FixedUpdate() {
        base.FixedUpdate();

        SetCooldownVisual();
        // Handle primary fire:
        if (primaryFireCooldownTimer > 0f) {
            primaryFireCooldownTimer -= Time.fixedDeltaTime;
        } else if (primaryInputStatus == WeaponSystem.InputStatus.Pressed && ConsumeAmmo(primaryAmmoCost)) {
            FireProjectile(primaryProjectilePrefab);
            primaryFireCooldownTimer = primaryFireCooldown;
        }

        // Handle secondary fire:
        if (secondaryireCooldownTimer > 0f) {
            secondaryireCooldownTimer -= Time.fixedDeltaTime;
            return;
        } else if (secondaryInputStatus == WeaponSystem.InputStatus.Pressed && ConsumeAmmo(secondaryAmmoCost)) {
            FireProjectile(secondaryProjectilePrefab);
            secondaryireCooldownTimer = secondaryFireCooldown;
        }
    }

    private GameObject FireProjectile(GameObject projectilePrefab) {
        RaycastProjectile newProjectile = Instantiate(projectilePrefab).GetComponent<RaycastProjectile>();
        newProjectile.transform.position = firePoint.position;
        newProjectile.Direction = ParentWeaponSystem.AimDirection;
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null) {
            newProjectile.InheritedVelocity = rb.linearVelocity;
        }

        return newProjectile.gameObject;
    }

    protected void SetCooldownVisual() {
        // The disappearance of the singularity will indicate when it's ready to fire again.
        mainSpriteRenderer.color = weaponColors.cooldownColorGradient.Evaluate(primaryFireCooldownTimer / primaryFireCooldown);
    }
}
