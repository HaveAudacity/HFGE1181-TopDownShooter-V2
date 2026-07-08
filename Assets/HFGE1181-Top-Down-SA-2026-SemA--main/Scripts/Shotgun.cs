using UnityEngine;

public class Shotgun : WeaponBase
{
    [SerializeField] private int pelletCount = 6;
    [SerializeField] private float spreadAngle = 15f;

    protected override void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Quaternion pelletRotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);
            Instantiate(bulletPrefab, firePoint.position, pelletRotation);
        }
    }
}
