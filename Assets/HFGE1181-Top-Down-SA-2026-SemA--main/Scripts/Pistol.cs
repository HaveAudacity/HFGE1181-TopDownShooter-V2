using UnityEngine;

public class Pistol : WeaponBase
{
    protected override void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Debug.DrawRay(firePoint.position, firePoint.up * 3f, Color.red, 2f);

        Debug.Log("Spawned bullet at: " + bullet.transform.position);
        Debug.Log("FirePoint rotation: " + firePoint.eulerAngles);
    }
}