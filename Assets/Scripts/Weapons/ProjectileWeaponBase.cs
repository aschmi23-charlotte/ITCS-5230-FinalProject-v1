using UnityEngine;

public class ProjectileWeaponBase : WeaponBase {

    [Header("Projectile Weapon Common")]
    public bool infiniteAmmo = false;
    public int ammoCount = 10;
    public int ammoCapacity = 10;

    public Transform firePoint;

}
