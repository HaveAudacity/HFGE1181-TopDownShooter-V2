using UnityEngine;
using System.Collections;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected int maxAmmo = 6;
    [SerializeField] protected float reloadTime = 1.5f;
    [SerializeField] protected GameObject bulletPrefab;

    [SerializeField] public Transform firePoint;

    [Header("Animator")]
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

        UIManager.Instance.UpdateReloadProgress(1f);
    }

    public void TryShoot()
    {
        if (Time.time < nextFireTime || isReloading || currentAmmo <= 0)
            return;

        Shoot();
        AudioManager.Instance.Play("ShootWeapon");
        WeaponData weaponData = this.GetComponent<WeaponData>();
        playerAnimator.SetTrigger(weaponData.shootTrigger);
        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        UIManager.Instance.UpdateReloadProgress((float)currentAmmo / maxAmmo);

        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);

        if (currentAmmo <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (!isReloading)
            StartCoroutine(ReloadCoroutine());
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        float timer = 0f;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            UIManager.Instance.UpdateReloadProgress(timer / reloadTime);
            yield return null;
        }

        currentAmmo = maxAmmo;
        isReloading = false;

        UIManager.Instance.UpdateReloadProgress(1f);
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }

    protected abstract void Shoot();
}
