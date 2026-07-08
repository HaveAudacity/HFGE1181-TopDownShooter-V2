using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Transforms")]
    public Transform weaponParent;
    [HideInInspector] public Transform weaponFirePoint;

    [Header("Current Weapon Info")]
    public GameObject currentWeaponGameObject;
    public GameObject currentPickup;

    [Header("Drop Settings")]
    public float dropForce = 5f;
    public float randomSpinForce = 300f;
    public float dropMoveDuration = 1f;
    public float pickupDelay = 10f;

    private WeaponBase currentWeaponScript;
    private PlayerHealth playerHealth;
    private Animator playerAnimator;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<Animator>();

        if (currentWeaponGameObject != null)
        {
            if (currentWeaponGameObject.transform.parent != weaponParent)
            {
                currentWeaponGameObject = Instantiate(currentWeaponGameObject, weaponParent.position, weaponParent.rotation, weaponParent);
            }
            else
            {
                currentWeaponGameObject.transform.localPosition = Vector3.zero;
                currentWeaponGameObject.transform.localRotation = Quaternion.identity;
            }

            currentWeaponScript = currentWeaponGameObject.GetComponent<WeaponBase>();

            WeaponData weaponData = currentWeaponGameObject.GetComponent<WeaponData>();
            if (weaponData != null)
            {
                currentPickup = weaponData.pickupPrefab;

               
            }
        }
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (playerHealth.isPlayerDead)
        {
            return;
        }

        if (context.performed && currentWeaponScript != null)
        {
            currentWeaponScript.TryShoot();
        }
    }

    public void SwapWeapon(GameObject newWeaponPrefab, GameObject newPickupPrefab)
    {
        if (currentWeaponGameObject != null)
        {
            WeaponData weaponData = currentWeaponGameObject.GetComponent<WeaponData>();
            if (weaponData != null && weaponData.pickupPrefab != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector3 dropPos = transform.position + (Vector3)randomDir * 2f;

                GameObject droppedWeapon = Instantiate(weaponData.pickupPrefab, dropPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

                Collider2D dropCollider = droppedWeapon.GetComponent<Collider2D>();
                if (dropCollider != null)
                    StartCoroutine(EnablePickupAfterDelay(dropCollider, pickupDelay));

                Rigidbody2D rb = droppedWeapon.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-randomSpinForce, randomSpinForce), ForceMode2D.Impulse);
                    StartCoroutine(StopDropMovement(rb));
                }
            }

            Destroy(currentWeaponGameObject);
        }

        currentWeaponGameObject = Instantiate(newWeaponPrefab, weaponParent.position, weaponParent.rotation, weaponParent);
        EquipWeapon(currentWeaponGameObject);
    }

    private void EquipWeapon(GameObject weaponPrefab)
    {
        currentWeaponScript = weaponPrefab.GetComponent<WeaponBase>();

        WeaponData weaponData = weaponPrefab.GetComponent<WeaponData>();
        if (weaponData != null)
        {
            currentPickup = weaponData.pickupPrefab;

            playerAnimator.SetTrigger(weaponData.idleTrigger);

           
        }

        Transform firePointTransform = weaponPrefab.transform.Find("WeaponFirePoint");
        if (firePointTransform != null)
        {
            weaponFirePoint = firePointTransform;
            currentWeaponScript.firePoint = weaponFirePoint;
        }
    }

    private IEnumerator StopDropMovement(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(dropMoveDuration);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private IEnumerator EnablePickupAfterDelay(Collider2D collider, float delay)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(delay);
        if (collider != null)
            collider.enabled = true;
    }
}
