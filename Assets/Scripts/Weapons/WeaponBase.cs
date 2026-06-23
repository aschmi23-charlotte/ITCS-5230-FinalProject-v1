using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {

    [SerializeField] protected WeaponSystem.InputStatus primaryInputStatus = WeaponSystem.InputStatus.NoInput;
    [SerializeField] protected WeaponSystem.InputStatus secondaryInputStatus = WeaponSystem.InputStatus.NoInput;

    [Header("Common Visuals")]
    // I'm not automatically detecting this in case a weapon has multiple sprite renderers.
    [SerializeField] protected SpriteRenderer mainSpriteRenderer;

    [Header("Common Ammo")]
    [SerializeField] public bool infiniteAmmo = false;
    [SerializeField] public int ammoCount = 20;
    [SerializeField] public int ammoCapacity = 20;
    [SerializeField] public float ammoRecoveryTimeBetweenTicks = 0.5f;
    [SerializeField] public float ammoRecoveryDelay = 1f;

    public WeaponSystem ParentWeaponSystem { get; private set; }

    protected float ammoRecoveryTickTimer = 0f;
    protected float ammoRecoveryDelayTimer = 0f;

    public WeaponSystem.InputStatus PrimaryInputStatus {
        get {
            return primaryInputStatus;
        }
        set {
            primaryInputStatus = value;
        }
    }

    public WeaponSystem.InputStatus SecondaryInputStatus {
        get {
            return secondaryInputStatus;
        }
        set {
            secondaryInputStatus = value;
        }
    }

    protected virtual void Awake() {
        ParentWeaponSystem = GetComponentInParent<WeaponSystem>();
    }

    // I'd rather enforce the pattern of having these be overrides now in-case changes are needed down the line.
    protected virtual void Start() {}
    protected virtual void Update() {}

    protected virtual void FixedUpdate() {
        // Handle ammo recovery:
        if (ammoRecoveryDelayTimer < ammoRecoveryDelay) {
            ammoRecoveryDelayTimer += Time.fixedDeltaTime;
        } else if (ammoRecoveryTickTimer < ammoRecoveryTimeBetweenTicks) {
            ammoRecoveryTickTimer += Time.fixedDeltaTime;
        } else if (ammoCount < ammoCapacity){
            ammoCount += 1;
            ammoRecoveryTickTimer = 0f;
        }
    }

    public bool IsActiveWeapon() {
        return ParentWeaponSystem.GetActiveWeapon() == this;
    }

    public virtual void OnSwitchTo() {
        mainSpriteRenderer.enabled = true;
    }

    public virtual void OnSwitchFrom() {
        mainSpriteRenderer.enabled = false;
        primaryInputStatus = WeaponSystem.InputStatus.NoInput;
        secondaryInputStatus = WeaponSystem.InputStatus.NoInput;
    }

    public bool ConsumeAmmo(int ammoAmount) {
        if (infiniteAmmo) {
            return true;
        }

        if (ammoCount - ammoAmount < 0f) {
            return false;
        }

        ammoCount -= ammoAmount;
        ammoRecoveryTickTimer = 0f;
        ammoRecoveryDelayTimer = 0f;
        
        return true;
    }
}
