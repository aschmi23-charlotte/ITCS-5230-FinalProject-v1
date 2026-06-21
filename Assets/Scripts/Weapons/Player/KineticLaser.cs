using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class KineticLaser : ProjectileWeaponBase {

    [System.Serializable]
    public enum WeaponState {
        Idle,
        PrimaryFireBasic,
        PrimaryFireChargingSuper,
        PrimaryFireSuperReady,
        PrimaryFireSuper,
        SecondaryFireActive,
        SecondaryFireComboReady,
        ComboFire,
        Cooldown,
    }

    [System.Serializable]
    protected class WeaponColors {
        public Color idleColor;
        public Color shotColor;
        public Gradient chargeColorGradient;
        public Gradient cooldownColorGradient;
    }

    [Header("Hit Information")]
    [SerializeField] float basicShotDamage = 10f;
    [SerializeField] float superShotDamage = 20f;
    [SerializeField] float tractorBeamStunPerSecond = 50f;

    [Header("Ammo Consumption")]
    [SerializeField] int basicShotAmmoCost = 1;
    [SerializeField] int superShotAmmoCost = 3;

    [Header("Kinetic Laser Timings")]
    [SerializeField] float basicPrimaryFireTime = 0.1f;
    [SerializeField] float basicPrimaryCooldownTime = 0.1f;    
    [SerializeField] float superChargeTime = 1.5f;
    [SerializeField] float superFireTime = 0.1f;
    [SerializeField] float superCooldownTime = 1f;
    [SerializeField] float basicSecondaryCooldownTime = 0.1f;
    [SerializeField] float basicSecondaryCollisionPauseTime = 0.5f;
    [SerializeField] float comboChargeTime = 1.5f;
    [SerializeField] float comboFireTime = 0.1f;
    [SerializeField] float comboCooldownTime = 1f;

    [Header("Kinetic Laser Visuals")]
    [SerializeField] WeaponColors weaponColors;
    [SerializeField] GameObject basicVisualPrefab;
    [SerializeField] GameObject superVisualPrefab;
    [SerializeField] GameObject tractorVisualPrefab;

    [Header("Kinetic Laser Impacts")]
    [SerializeField] ContactFilter2D impactFilter;
    [SerializeField] float basicForce = 25f;
    [SerializeField] float superForce = 45f;
    [SerializeField] float tractorStartForce = 50f;
    [SerializeField] float tractorHoldForce = 30f;
    [SerializeField] bool applyForcesAtImpactPoint = true;
    [SerializeField] float tractorBeamMinimumDistance = 1f;

    // Internals
    [Header("Internals")]
    [SerializeField] WeaponState weaponState = WeaponState.Idle;
    [SerializeField] float cooldownTarget = 0f;
    [SerializeField] float stateLifeTimer = 0f;

    protected LineRenderer laserVisual = null;
    protected RaycastHit2D[] impactResults = new RaycastHit2D[5];
    protected Collider2D wielderCollider;
    protected float secondaryShortoutTimer = 0.0f;

    protected override void Awake() {
        base.Awake();
        wielderCollider = GetComponentInParent<Collider2D>();
    }

    protected override void Update() {
        if (weaponState == WeaponState.PrimaryFireBasic) {
            UpdateBasicLaserShotRendering();
        } else if (weaponState == WeaponState.PrimaryFireSuper || weaponState == WeaponState.ComboFire) {
            UpdateSuperLaserShotRendering();
        } else if (weaponState == WeaponState.SecondaryFireActive || weaponState == WeaponState.SecondaryFireComboReady) {
            UpdateTractorBeamRendering();
        }
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        UpdateWeaponState();
        UpdateWeaponColor();
    }

    void UpdateWeaponState() {
        stateLifeTimer += Time.fixedDeltaTime;
        if (secondaryShortoutTimer > 0f) {
            secondaryShortoutTimer -= Time.fixedDeltaTime;
        }
        
        if (weaponState == WeaponState.Idle) {
            // Weapon is ready
            if (primaryInputStatus == WeaponSystem.InputStatus.Pressed && ConsumeAmmo(basicShotAmmoCost)) {
                StartBasicLaserShot();
                ChangeWeaponState(WeaponState.PrimaryFireBasic);

            } else if (secondaryInputStatus == WeaponSystem.InputStatus.Pressed || secondaryInputStatus == WeaponSystem.InputStatus.Held) {
                StartTractorBeam();
                ChangeWeaponState(WeaponState.SecondaryFireActive);
            }

        } else if (weaponState == WeaponState.PrimaryFireBasic) {
            // Primary Fire
            if (stateLifeTimer >= basicPrimaryFireTime) {
                EndBasicLaserShot();
                if (primaryInputStatus == WeaponSystem.InputStatus.Held) {
                    // Start Charging
                    ChangeWeaponState(WeaponState.PrimaryFireChargingSuper);
                } else {
                    // No Charging
                    StartCooldown(basicPrimaryCooldownTime);
                }
            }
        } else if (weaponState == WeaponState.PrimaryFireChargingSuper) {
            ammoRecoveryDelayTimer = 0f;
            if (primaryInputStatus == WeaponSystem.InputStatus.Released || primaryInputStatus == WeaponSystem.InputStatus.NoInput) {
                // End Charging Early
                StartCooldown(basicPrimaryCooldownTime);
            } else if (stateLifeTimer >= superChargeTime) {
                // Charge Complete
                ChangeWeaponState(WeaponState.PrimaryFireSuperReady);
            }

        } else if (weaponState == WeaponState.PrimaryFireSuperReady) {
            ammoRecoveryDelayTimer = 0f;
            // Ready to Fire Super
            if (primaryInputStatus == WeaponSystem.InputStatus.Released && ConsumeAmmo(superShotAmmoCost)) {
                // Fire Super
                StartSuperLaserShot();
                ChangeWeaponState(WeaponState.PrimaryFireSuper);
            } else if (primaryInputStatus == WeaponSystem.InputStatus.NoInput) {
                StartCooldown(basicPrimaryCooldownTime);
            }

        } else if (weaponState == WeaponState.PrimaryFireSuper) {
            // Firing Super
            if (stateLifeTimer >= superFireTime) {
                // Fire Complete
                EndSuperLaserShot();
                StartCooldown(superCooldownTime);    
            }

        } else if (weaponState == WeaponState.SecondaryFireActive) {
            SetChargeVisual(stateLifeTimer / comboChargeTime);
            ammoRecoveryDelayTimer = 0f;
            if (stateLifeTimer >= comboChargeTime && secondaryInputStatus == WeaponSystem.InputStatus.Held) {
                // Combo Ready
                ChangeWeaponState(WeaponState.SecondaryFireComboReady);
            } else if (
                secondaryInputStatus == WeaponSystem.InputStatus.Released
                || secondaryInputStatus == WeaponSystem.InputStatus.NoInput
            ) {
                // End Secondary (User Input)
                EndTractorBeam();
                StartCooldown(basicSecondaryCooldownTime);
            }

        } else if (weaponState == WeaponState.SecondaryFireComboReady) {
            ammoRecoveryDelayTimer = 0f;
            if (
                secondaryInputStatus == WeaponSystem.InputStatus.Released
                || secondaryInputStatus == WeaponSystem.InputStatus.NoInput
            ) {
                // End Secondary (User Input)
                EndTractorBeam();
                StartCooldown(basicSecondaryCooldownTime);
            } else if (primaryInputStatus == WeaponSystem.InputStatus.Pressed && ConsumeAmmo(superShotAmmoCost)) {
                EndTractorBeam();
                StartSuperLaserShot();
                ChangeWeaponState(WeaponState.ComboFire);
            }

        } else if (weaponState == WeaponState.ComboFire) {
            // Fire Combo
            if (stateLifeTimer >= comboFireTime) {
                EndSuperLaserShot();
                StartCooldown(comboCooldownTime);
            }

        } else if (weaponState == WeaponState.Cooldown) {
            // Cooldown Handling
            if (stateLifeTimer >= cooldownTarget) {
                ChangeWeaponState(WeaponState.Idle);
            }
        }
    }

    void UpdateWeaponColor() {
        if (weaponState == WeaponState.Idle) {
            mainSpriteRenderer.color = weaponColors.idleColor;

        } else if (weaponState == WeaponState.PrimaryFireBasic) {
            mainSpriteRenderer.color = weaponColors.shotColor;

        } else if (weaponState == WeaponState.PrimaryFireChargingSuper) {
            SetChargeVisual(stateLifeTimer / superChargeTime);

        } else if (weaponState == WeaponState.PrimaryFireSuperReady) {
            SetChargeVisual(1f);

        } else if (weaponState == WeaponState.PrimaryFireSuper) {
            mainSpriteRenderer.color = weaponColors.shotColor;

        } else if (weaponState == WeaponState.SecondaryFireActive) {
            SetChargeVisual(stateLifeTimer / comboChargeTime);

        } else if (weaponState == WeaponState.SecondaryFireComboReady) {
            SetChargeVisual(1f);

        } else if (weaponState == WeaponState.ComboFire) {
            mainSpriteRenderer.color = weaponColors.shotColor;

        } else if (weaponState == WeaponState.Cooldown) {
            SetCooldownVisual(1f - (stateLifeTimer / cooldownTarget));
        }
    }

    protected void SetChargeVisual(float chargePercent) {
        mainSpriteRenderer.color = weaponColors.chargeColorGradient.Evaluate(chargePercent);
    }

    protected void SetCooldownVisual(float coolPercent) {
        mainSpriteRenderer.color = weaponColors.cooldownColorGradient.Evaluate(coolPercent);
    }

    protected void StartBasicLaserShot() {
        int hitscan = Physics2D.Raycast((Vector2)firePoint.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(basicVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = firePoint.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);

        if (impactResults[1].rigidbody != null) {
            if (applyForcesAtImpactPoint) {
                impactResults[1].rigidbody.AddForceAtPosition(ParentWeaponSystem.AimDirection * basicForce, impactResults[1].point, ForceMode2D.Impulse);
            } else {
                impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * basicForce, ForceMode2D.Impulse);
            }
        }

        Hurtbox hurtbox = impactResults[1].collider.GetComponent<Hurtbox>();
        if (hurtbox != null) {
            hurtbox.RecieveDamageHit(basicShotDamage);
        }
    }

    protected void UpdateBasicLaserShotRendering() {
        laserVisual.transform.position = firePoint.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);
    }

    protected void EndBasicLaserShot() {
        Destroy(laserVisual.gameObject);
    }

    protected void StartSuperLaserShot() {
        int hitscan = Physics2D.Raycast((Vector2)firePoint.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(superVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = firePoint.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);

        if (impactResults[1].rigidbody != null) {
            if (applyForcesAtImpactPoint) {
                impactResults[1].rigidbody.AddForceAtPosition(ParentWeaponSystem.AimDirection * superForce, impactResults[1].point, ForceMode2D.Impulse);
            } else {
                impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * superForce, ForceMode2D.Impulse);
            }
        }

        Hurtbox hurtbox = impactResults[1].collider.GetComponent<Hurtbox>();
        if (hurtbox != null) {
            hurtbox.RecieveDamageHit(superShotDamage);
        }

    }

    protected void UpdateSuperLaserShotRendering() {
        laserVisual.transform.position = firePoint.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);
    }

    protected void EndSuperLaserShot() {
        Destroy(laserVisual.gameObject);
    }

    bool ShouldTractorBeamPause() {
        return impactResults[1].distance < tractorBeamMinimumDistance || Physics2D.IsTouching(impactResults[1].collider, wielderCollider);
    }

    protected void StartTractorBeam() {
        int hitscan = Physics2D.Raycast((Vector2)firePoint.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(tractorVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = firePoint.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);
        if (ShouldTractorBeamPause()) {
            secondaryShortoutTimer = basicSecondaryCollisionPauseTime;
        }

        if (secondaryShortoutTimer <= 0f && impactResults[1].rigidbody != null) {
            if (applyForcesAtImpactPoint) {
                impactResults[1].rigidbody.AddForceAtPosition(ParentWeaponSystem.AimDirection * -tractorStartForce, impactResults[1].point, ForceMode2D.Force);
            } else {
                impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorStartForce, ForceMode2D.Force);
            }
        }

    }

    protected void UpdateTractorBeamRendering() {
        int hitscan = Physics2D.Raycast((Vector2)firePoint.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);
        laserVisual.transform.position = firePoint.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)firePoint.position);

        if (ShouldTractorBeamPause()) {
            secondaryShortoutTimer = basicSecondaryCollisionPauseTime;
        }

        if (secondaryShortoutTimer <= 0f && impactResults[1].rigidbody != null) {
            if (applyForcesAtImpactPoint) {
                impactResults[1].rigidbody.AddForceAtPosition(ParentWeaponSystem.AimDirection * -tractorHoldForce, impactResults[1].point, ForceMode2D.Force);
            } else {
                impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorHoldForce, ForceMode2D.Force);
            }
        }

        // I'm trying to keep all game logic and interactions on FixedUpdate, to make pausing easy.
        // And Update should only call rendering-related code.
        // This is being called by Update, so I probably need to move it... later.
        Hurtbox hurtbox = impactResults[1].collider.GetComponent<Hurtbox>();
        if (hurtbox != null) {
            hurtbox.RecieveStunHit(tractorBeamStunPerSecond * Time.deltaTime, false);
        }
    }

    protected void EndTractorBeam() {
        Destroy(laserVisual.gameObject);
    }

    private void StartCooldown(float cooldownTime) {
        cooldownTarget = cooldownTime;
        ChangeWeaponState(WeaponState.Cooldown);
    }

    private void ChangeWeaponState(WeaponState newState) {
        weaponState = newState;
        stateLifeTimer = 0f;
    }
}
