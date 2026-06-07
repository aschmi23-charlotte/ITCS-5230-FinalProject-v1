using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {

    [SerializeField]
    protected WeaponSystem.InputStatus primaryInputStatus = WeaponSystem.InputStatus.NoInput;

    [SerializeField]
    protected WeaponSystem.InputStatus secondaryInputStatus = WeaponSystem.InputStatus.NoInput;


    public WeaponSystem ParentWeaponSystem { get; private set; }

    protected void Awake() {
        ParentWeaponSystem = GetComponentInParent<WeaponSystem>();
    }

    public virtual void ForceStateReset() { }

    public void SetPrimaryInputStatus(WeaponSystem.InputStatus status) {
        primaryInputStatus = status;
    }

    public void SetSecondaryInputStatus(WeaponSystem.InputStatus status) {
        secondaryInputStatus = status;
    }
}
