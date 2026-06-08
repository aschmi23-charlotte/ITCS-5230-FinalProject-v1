using UnityEngine;

public class ProjectileWeaponBase : WeaponBase {

    [Header("Projectile Weapon Common")]
    [SerializeField] bool infiniteAmmo = false;
    [SerializeField] int ammoCount = 10;
    [SerializeField] int ammoCapacity = 10;

    [SerializeField] Transform firePoint;

}
