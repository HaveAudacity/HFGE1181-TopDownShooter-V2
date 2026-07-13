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
                Debug.Log("Equipped: " + currentWeaponGameObject.name);
            }

            else
            {
                currentWeaponGameObject.transform.localPosition = Vector3.zero;
                currentWeaponGameObject.transform.localRotation = Quaternion.identity;
            }

            EquipWeapon(currentWeaponGameObject);
            Debug.Log("Current Weapon Script: " + currentWeaponScript);
        }
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        Debug.Log("Shoot pressed");
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
        Debug.Log("=== NEW SWAPWEAPON CODE RUNNING ===");

        if (currentWeaponGameObject != null)
        {
            Debug.Log("Current weapon: " + currentWeaponGameObject.name);

            WeaponData weaponData = currentWeaponGameObject.GetComponent<WeaponData>();
            Debug.Log("WeaponData = " + weaponData);

            if (weaponData != null)
            {
                Debug.Log("Pickup prefab = " + weaponData.pickupPrefab);
            }

            if (weaponData != null && weaponData.pickupPrefab != null)
            {
                Debug.Log("Dropping: " + weaponData.pickupPrefab.name);

                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector3 dropPos = transform.position + (Vector3)randomDir * 2f;

                GameObject droppedWeapon = Instantiate(
                    weaponData.pickupPrefab,
                    dropPos,
                    Quaternion.Euler(0, 0, Random.Range(0f, 360f))
                );

                Debug.Log("Dropped weapon created: " + droppedWeapon.name);

                Collider2D dropCollider = droppedWeapon.GetComponent<Collider2D>();
                Debug.Log("Drop collider = " + dropCollider);

                if (dropCollider != null)
                {
                    dropCollider.enabled = false;
                    StartCoroutine(EnablePickupAfterDelay(dropCollider, pickupDelay));
                }

                Rigidbody2D rb = droppedWeapon.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-randomSpinForce, randomSpinForce), ForceMode2D.Impulse);
                    StartCoroutine(StopDropMovement(rb));
                }
            }

            Debug.Log("Destroying current weapon");
            Destroy(currentWeaponGameObject);
        }

        Debug.Log("Instantiating: " + newWeaponPrefab.name);

        currentWeaponGameObject = Instantiate(
            newWeaponPrefab,
            weaponParent.position,
            weaponParent.rotation,
            weaponParent
        );

        Debug.Log("New weapon object: " + currentWeaponGameObject.name);

        EquipWeapon(currentWeaponGameObject);

        Debug.Log("Weapon script = " + currentWeaponScript);
        Debug.Log("=== SwapWeapon END ===");
    }

    private void EquipWeapon(GameObject weaponPrefab)
    {
        currentWeaponScript = weaponPrefab.GetComponent<WeaponBase>();

        WeaponData weaponData = weaponPrefab.GetComponent<WeaponData>();
        if (weaponData != null)
        {
            currentPickup = weaponData.pickupPrefab;

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(weaponData.idleTrigger);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateWeaponIcon(weaponData.weaponIcon);
            }
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
        Debug.Log("Pickup disabled");

        collider.enabled = false;

        yield return new WaitForSeconds(delay);

        Debug.Log("Pickup enabled");

        if (collider != null)
            collider.enabled = true;
    }
}
