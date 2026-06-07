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
    public ContactFilter2D impactFilter;
    public GameObject basicVisualPrefab;
    public GameObject superVisualPrefab;
    public GameObject tractorVisualPrefab;
    public float basicForce = 25f;
    public float superForce = 45f;
    public float tractorForce = 50f;

    [Header("Kinetic Laser Timings")]
    public float basicPrimaryFireTime = 0.1f;
    public float basicPrimaryCooldownTime = 0.1f;
    public float superChargeTime = 1.5f;
    public float superFireTime = 0.1f;
    public float superCooldownTime = 1f;

    public float basicSecondaryCooldownTime = 0.1f;
    public float comboChargeTime = 1.5f;
    public float comboFireTime = 0.1f;
    public float comboCooldownTime = 1f;

    // Internals
    [Header("Internals")]
    public WeaponState weaponState = WeaponState.Idle;
    public float cooldownTarget = 0f;
    public float stateLifeTimer = 0f;

    protected LineRenderer laserVisual = null;
    protected RaycastHit2D[] impactResults = new RaycastHit2D[5];

    // Primary State Management Loop:
    void FixedUpdate() {
        stateLifeTimer += Time.fixedDeltaTime;
        
        if (weaponState == WeaponState.Idle) {
            // Weapon is ready
            if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
                StartBasicLaserShot();
                ChangeWeaponState(WeaponState.PrimaryFireBasic);

            } else if (secondaryInputStatus == WeaponSystem.InputStatus.Pressed) {
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
            } else {
                UpdateBasicLaserShot();
            }

        } else if (weaponState == WeaponState.PrimaryFireChargingSuper) {
            if (primaryInputStatus == WeaponSystem.InputStatus.Released) {
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
            }

        } else if (weaponState == WeaponState.PrimaryFireSuper) {
            // Firing Super
            if (stateLifeTimer >= superFireTime) {
                // Fire Complete
                EndSuperLaserShot();
                StartCooldown(superCooldownTime);    
            } else {
                UpdateSuperLaserShot();
            }

        } else if (weaponState == WeaponState.SecondaryFireActive) {
            if (stateLifeTimer >= comboChargeTime && secondaryInputStatus == WeaponSystem.InputStatus.Held) {
                // Combo Ready
                ChangeWeaponState(WeaponState.SecondaryFireComboReady);
            } else if (secondaryInputStatus == WeaponSystem.InputStatus.Released) {
                EndTractorBeam();
                // End Secondary
                StartCooldown(basicSecondaryCooldownTime);
            } else {
                UpdateTractorBeam();
            }

        } else if (weaponState == WeaponState.SecondaryFireComboReady) {
            if (secondaryInputStatus == WeaponSystem.InputStatus.Released) {
                EndTractorBeam();
                // End Secondary
                StartCooldown(basicSecondaryCooldownTime);
            } else if (primaryInputStatus == WeaponSystem.InputStatus.Pressed) {
                EndTractorBeam();
                ChangeWeaponState(WeaponState.ComboFire);
            } else {
                UpdateTractorBeam();
            }

        } else if (weaponState == WeaponState.ComboFire) {
            // Fire Combo
            if (stateLifeTimer >= comboFireTime) {
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

    protected void StartTractorBeam() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);

        // Pooling would be more efficient, but w/e.
        laserVisual = Instantiate(tractorVisualPrefab).GetComponent<LineRenderer>();
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.positionCount = 2;
        laserVisual.SetPosition(0, Vector2.zero);
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);

        if (impactResults[1].rigidbody != null) {
            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorForce, ForceMode2D.Force);
        }

    }

    protected void UpdateTractorBeam() {
        int hitscan = Physics2D.Raycast((Vector2)ParentWeaponSystem.aimOrigin.position, ParentWeaponSystem.AimDirection, impactFilter, impactResults);
        laserVisual.transform.position = ParentWeaponSystem.aimOrigin.position;
        laserVisual.SetPosition(1, impactResults[1].point - (Vector2)ParentWeaponSystem.aimOrigin.position);
        if (impactResults[1].rigidbody != null) {

            impactResults[1].rigidbody.AddForce(ParentWeaponSystem.AimDirection * -tractorForce, ForceMode2D.Force);
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
