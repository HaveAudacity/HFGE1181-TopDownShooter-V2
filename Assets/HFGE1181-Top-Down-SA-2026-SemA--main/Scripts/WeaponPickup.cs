using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject weaponGameObjectPrefab;
    public GameObject pickupPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWeaponManager playerWeapon = collision.GetComponent<PlayerWeaponManager>();
        if (playerWeapon != null)
        {
            playerWeapon.SwapWeapon(weaponGameObjectPrefab, pickupPrefab);
            AudioManager.Instance.Play("WeaponPickup");
            Destroy(gameObject);
        }
    }
}
