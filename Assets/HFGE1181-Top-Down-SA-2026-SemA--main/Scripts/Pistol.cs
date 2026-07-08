using UnityEngine;

public class Pistol : WeaponBase
{
    protected override void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
