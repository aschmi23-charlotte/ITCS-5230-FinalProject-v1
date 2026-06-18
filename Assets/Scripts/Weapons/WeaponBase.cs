using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {

    [SerializeField] protected WeaponSystem.InputStatus primaryInputStatus = WeaponSystem.InputStatus.NoInput;
    [SerializeField] protected WeaponSystem.InputStatus secondaryInputStatus = WeaponSystem.InputStatus.NoInput;

    [Header("Common Visuals")]
    [SerializeField] protected SpriteRenderer mainSpriteRenderer;

    [Header("Ammo")]
    [SerializeField] public bool infiniteAmmo = false;
    [SerializeField] public int ammoCount = 10;
    [SerializeField] public int ammoCapacity = 10;

    public WeaponSystem ParentWeaponSystem { get; private set; }

    protected virtual void Awake() {
        ParentWeaponSystem = GetComponentInParent<WeaponSystem>();
    }

    public void SetPrimaryInputStatus(WeaponSystem.InputStatus status) {
        primaryInputStatus = status;
    }

    public void SetSecondaryInputStatus(WeaponSystem.InputStatus status) {
        secondaryInputStatus = status;
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

    // I'm not sure this will actually be used, but I'll hold out until health stuff is implemented.
    public virtual void ForceStateReset() { }
}
