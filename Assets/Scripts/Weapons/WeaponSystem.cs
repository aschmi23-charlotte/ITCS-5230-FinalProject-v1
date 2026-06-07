using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponSystem : MonoBehaviour {
    [System.Serializable]
    public enum AimType {
        Direction,
        Position
    }

    // Editor Fields
    [Header("Weapon Handling")]
    public AimType aimType = AimType.Direction;
    public Transform aimOrigin;
    public SpriteRenderer aimGraphic;

    [Header("Weapon Instances")]
    public List<WeaponBase> weapons;


    public Vector2 AimDirection { get; private set; }
    public Vector2 AimPosition { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }
    
    // Mostly used with Gamepad control
    public void SetAimByDirection(Vector2 direction) {
        aimType = AimType.Direction;
        AimDirection = direction.normalized;
        Vector3 aimOriginGlobalPos = transform.TransformPoint(aimOrigin.position);
        AimPosition = new Vector2(aimOriginGlobalPos.x, aimOriginGlobalPos.y) + AimDirection;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
        aimOrigin.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetAimByPosition(Vector2 position) {
        aimType = AimType.Position;
        AimPosition = position;
        Vector3 aimOriginGlobalPos = transform.TransformPoint(aimOrigin.position);
        AimDirection = (position - new Vector2(aimOriginGlobalPos.x, aimOriginGlobalPos.y)).normalized;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(AimDirection.y, AimDirection.x);
        aimOrigin.rotation = Quaternion.Euler(0, 0, angle);
    }
}
