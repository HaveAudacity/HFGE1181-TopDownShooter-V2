using UnityEngine;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected int maxAmmo = 6;
    [SerializeField] protected float reloadTime = 1.5f;
    [SerializeField] protected GameObject bulletPrefab;

    public Transform firePoint;

    private Animator playerAnimator;

    protected int currentAmmo;
    protected float nextFireTime = 0f;
    protected bool isReloading = false;

    public System.Action<int, int> OnAmmoChanged;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);

        playerAnimator = GetComponentInParent<Animator>();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateReloadProgress(1f);
        }
    }

    public void TryShoot()
    {
        if (Time.time < nextFireTime || isReloading || currentAmmo <= 0)
            return;

        Shoot();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("ShootWeapon");
        }

        WeaponData weaponData = GetComponent<WeaponData>();

        if (playerAnimator != null && weaponData != null)
        {
            playerAnimator.SetTrigger(weaponData.shootTrigger);
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateReloadProgress((float)currentAmmo / maxAmmo);
        }

        if (currentAmmo <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (!isReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        float timer = 0f;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateReloadProgress(timer / reloadTime);
            }

            yield return null;
        }

        currentAmmo = maxAmmo;
        isReloading = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateReloadProgress(1f);
        }

        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    protected abstract void Shoot();
}