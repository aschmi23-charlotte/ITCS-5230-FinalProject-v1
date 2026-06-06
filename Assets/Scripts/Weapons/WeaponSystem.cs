using UnityEngine;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour {
    [System.Serializable]
    public enum AimType {
        Directional,
        Target
    }

    // Editor Fields
    [Header("Weapon Handling")]
    public AimType aimType = AimType.Directional;
    public Transform aimOrigin;
    public SpriteRenderer aimGraphic;

    [Header("Weapon Instances")]
    public List<WeaponBase> weapons;


    public Vector2 AimDirection { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }
    
    // Mostly used with Gamepad control
    public void SetAimByDirection(Vector2 direction) {
        AimDirection = direction.normalized;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        aimOrigin.rotation = Quaternion.Euler(0, 0, angle);
    }
}
