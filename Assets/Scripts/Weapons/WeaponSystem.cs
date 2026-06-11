using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponSystem : MonoBehaviour {
    [System.Serializable]
    public enum AimType {
        Direction,
        Position
    }

    [System.Serializable]
    public enum InputStatus {
        NoInput,
        Pressed,
        Held,
        Released,
    }

    // Editor Fields
    [Header("Weapon Handling")]
    public AimType aimType = AimType.Direction;
    public Transform aimOrigin;
    public SpriteRenderer aimGraphic;
    public float directionalAimingAutoDistance = 10f;

    [Header("Weapon Instances")]
    [SerializeField] private int activeWeaponIndex = 0;
    [SerializeField] private List<WeaponBase> weapons;
    
    public Vector2 AimDirection { get; private set; }
    public Vector2 AimPosition { get; private set; }

    public int ActiveWeaponIndex {
        get {
            return activeWeaponIndex;
        }
        set {
            weapons[activeWeaponIndex].OnSwitchFrom();
            activeWeaponIndex = value;
            weapons[activeWeaponIndex].OnSwitchTo();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        ResetWeapons();
    }

    void ResetWeapons() {
        foreach (WeaponBase weapon in weapons) {
            weapon.OnSwitchFrom();
        }

        weapons[activeWeaponIndex].OnSwitchTo();
    }

    public WeaponBase GetWeapon(int index) {
        return weapons[index];
    }

    public WeaponBase GetActiveWeapon() {
        return GetWeapon(activeWeaponIndex);
    }

    // Mostly used with Gamepad control
    public void SetAimByDirection(Vector2 direction) {
        aimType = AimType.Direction;
        AimDirection = direction.normalized;
        AimPosition = new Vector2(aimOrigin.position.x, aimOrigin.position.y) + AimDirection * directionalAimingAutoDistance;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(AimDirection.y, AimDirection.x);
        aimOrigin.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Mostly used with KB+M control
    public void SetAimByPosition(Vector2 position) {
        aimType = AimType.Position;
        AimPosition = position;
        AimDirection = (position - new Vector2(aimOrigin.position.x, aimOrigin.position.y)).normalized;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(AimDirection.y, AimDirection.x);
        aimOrigin.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetPrimaryInputStatus(InputStatus status) {
        weapons[activeWeaponIndex].SetPrimaryInputStatus(status);
    }

    public void SetSecondaryInputStatus(InputStatus status) {
        weapons[activeWeaponIndex].SetSecondaryInputStatus(status);
    }
}
