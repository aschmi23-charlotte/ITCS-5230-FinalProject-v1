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

    [Header("Kinetic Laser Config")]
    [SerializeField] ContactFilter2D impactFilter;
    [SerializeField] GameObject basicVisualPrefab;
    [SerializeField] GameObject superVisualPrefab;
    [SerializeField] GameObject tractorVisualPrefab;
    [SerializeField] float basicForce = 25f;
    [SerializeField] float superForce = 45f;
    [SerializeField] float tractorStartForce = 50f;
    [SerializeField] float tractorHoldForce = 30f;

    [Header("Kinetic Laser Timings")]
    [SerializeField] float basicPrimaryFireTime = 0.1f;
    [SerializeField] float basicPrimaryCooldownTime = 0.1f;
    [SerializeField] float superChargeTime = 1.5f;
    [SerializeField] float superFireTime = 0.1f;
    [SerializeField] float superCooldownTime = 1f;

    [SerializeField] float basicSecondaryCooldownTime = 0.1f;
    [SerializeField] float comboChargeTime = 1.5f;
    [SerializeField] float comboFireTime = 0.1f;
    [SerializeField] float comboCooldownTime = 1f;

    // Internals
    [Header("Internals")]
    [SerializeField] WeaponState weaponState = WeaponState.Idle;
    [SerializeField] float cooldownTarget = 0f;
    [SerializeField] float stateLifeTimer = 0f;

    protected LineRenderer laserVisual = null;
    protected RaycastHit2D[] impactResults = new RaycastHit2D[5];

    // Primary State Management Loop:
    private PlayerMovementHandler playerMovementHandler;
    void Start() {
        playerMovementHandler = GetComponentInParent<PlayerMovementHandler>();
    }

    void Update() {
        if (weaponState == WeaponState.PrimaryFireBasic) {
            UpdateBasicLaserShot();
        } else if (weaponState == WeaponState.PrimaryFireSuper || weaponState == WeaponState.ComboFire) {
            UpdateSuperLaserShot();
        } else if (weaponState == WeaponState.SecondaryFireActive || weaponState == WeaponState.SecondaryFireComboReady) {
            UpdateTractorBeam();
        }
    }


    void FixedUpdate() {
        stateLifeTimer += Time.fixedDeltaTime;
        
        if (weaponState == WeaponState.Idle) {
            // Weapon is ready
            if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
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
            if (primaryInputStatus == WeaponSystem.InputStatus.Released || primaryInputStatus == WeaponSystem.InputStatus.NoInput) {
                // End Charging Early
                StartCooldown(basicPrimaryCooldownTime);
            } else if (stateLifeTimer >= superChargeTime) {
                // Charge Complete
                ChangeWeaponState(WeaponState.PrimaryFireSuperReady);
            }

        } else if (weaponState == WeaponState.PrimaryFireSuperReady) {
            // Ready to Fire Super
            if (primaryInputStatus == WeaponSystem.InputStatus.Released) {
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
            if (stateLifeTimer >= comboChargeTime && secondaryInputStatus == WeaponSystem.InputStatus.Held) {
                // Combo Ready
                ChangeWeaponState(WeaponState.SecondaryFireComboReady);
            } else if (secondaryInputStatus == WeaponSystem.InputStatus.Released || secondaryInputStatus == WeaponSystem.InputStatus.NoInput) {
                EndTractorBeam();
                // End Secondary
                StartCooldown(basicSecondaryCooldownTime);
            }

        } else if (weaponState == WeaponState.SecondaryFireComboReady) {
            if (secondaryInputStatus == WeaponSystem.InputStatus.Released || secondaryInputStatus == WeaponSystem.InputStatus.NoInput) {
                EndTractorBeam();
                // End Secondary
                StartCooldown(basicSecondaryCooldownTime);
            } else if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
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

    protected void StartBasicLaserShot() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(basicVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);

        if (impactResults[1].rigidbody != null) {
            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * basicForce, ForceMode2D.Impulse);
        }
        
    }

    protected void UpdateBasicLaserShot() {
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);
    }

    protected void EndBasicLaserShot() {
        Destroy(laserVisual.gameObject);
    }

    protected void StartSuperLaserShot() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(superVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);

        if (impactResults[1].rigidbody != null) {
            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * superForce, ForceMode2D.Impulse);
        }

    }

    protected void UpdateSuperLaserShot() {
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);
    }

    protected void EndSuperLaserShot() {
        Destroy(laserVisual.gameObject);
    }

    bool isTractoredObjectHittingSelf() {
        return impactResults[1].collider == playerMovementHandler.GetGroundedCast();
    }

    protected void StartTractorBeam() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(tractorVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);

        if (impactResults[1].rigidbody != null && !isTractoredObjectHittingSelf()) {
            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorStartForce, ForceMode2D.Force);
        }

    }

    protected void UpdateTractorBeam() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);
        if (impactResults[1].rigidbody != null) {

            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorHoldForce, ForceMode2D.Force);
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
