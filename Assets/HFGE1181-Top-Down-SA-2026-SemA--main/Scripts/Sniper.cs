using UnityEngine;

public class Sniper : WeaponBase
{
    protected override void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.SetPiercing(true);
        }
    }
}
