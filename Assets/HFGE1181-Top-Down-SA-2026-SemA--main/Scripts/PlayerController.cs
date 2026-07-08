using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Knockback Settings")]
    private float knockbackDistanceLimit = 0.5f;
    private float knockbackSpeed = 15f;

    [Header("Animator")]
    private Animator playerAnimator;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private PlayerHealth playerHealth;
    private PlayerWeaponManager playerWeaponManager;

    private Vector2 moveInput;
    private Vector2 mousePosition;
    private bool isKnockedBack = false;
    private Vector2 knockbackDirection;
    private Vector2 knockbackStartPos;
    private bool isSprinting = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        playerHealth = GetComponent<PlayerHealth>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
        playerAnimator = GetComponentInParent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            playerAnimator.SetBool("IsSprinting", true);
            WeaponData weaponData = playerWeaponManager.currentWeaponGameObject.GetComponent<WeaponData>();
            playerAnimator.SetTrigger(weaponData.sprintTrigger);
        }
        else if (context.canceled)
        {
            isSprinting = false;
            playerAnimator.SetBool("IsSprinting", false);
        }
    }

    private void FixedUpdate()
    {
        RotateTowardsMouse();

        if (isKnockedBack)
        {
            float distTravelled = Vector2.Distance(rb.position, knockbackStartPos);
            if (distTravelled >= knockbackDistanceLimit)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.linearVelocity = knockbackDirection * knockbackSpeed;
                return;
            }
        }

        MoveRelativeToScreen();
    }

    private void RotateTowardsMouse()
    {
        if (playerHealth.isPlayerDead)
        {
            return;
        }

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector2 direction = mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private void MoveRelativeToScreen()
    {
        float speed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector2 moveDirection = moveInput;
        rb.linearVelocity = moveDirection.normalized * speed;
    }

    public void ApplyKnockback(Vector2 forceDirection)
    {
        isKnockedBack = true;
        knockbackStartPos = rb.position;
        knockbackDirection = forceDirection.normalized;
    }
}
