using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float minimumPlayerDistance = 0.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float stunDurationAfterKnockback = 0.5f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private string attackTriggerName = "Attack";

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerHealth playerHealth;

    private bool isKnockedBack;
    private bool isAttacking;
    private bool isStunned;

    private float fireRateTimer;
    private float knockbackTimer;
    private float stunTimer;

    private void Awake()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("Player not found.");
            enabled = false;
            return;
        }

        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (HandleKnockback())
            return;

        if (HandleStun())
            return;

        if (isAttacking)
        {
            fireRateTimer -= Time.deltaTime;

            if (fireRateTimer <= 0f)
            {
                AttackPlayer();
            }
        }

        if (player == null)
            return;

        if (CalculateDistance() > minimumPlayerDistance)
            RotateTowardsPlayer();
        else
            RotateAwayFromPlayer();

        Movement();
    }

    private bool HandleKnockback()
    {
        if (!isKnockedBack)
            return false;

        knockbackTimer -= Time.deltaTime;

        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            rb.linearVelocity = Vector2.zero;

            isStunned = true;
            stunTimer = stunDurationAfterKnockback;
        }

        if (animator != null)
            animator.SetBool("isWalking", false);

        return true;
    }

    private bool HandleStun()
    {
        if (!isStunned)
            return false;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            isStunned = false;
            return false;
        }

        if (animator != null)
            animator.SetBool("isWalking", false);

        return true;
    }

    private float CalculateDistance()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    private void Movement()
    {
        if (playerHealth != null && playerHealth.isPlayerDead)
        {
            if (animator != null)
                animator.SetBool("isWalking", false);

            return;
        }

        if (CalculateDistance() > attackRange)
        {
            isAttacking = false;

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime);

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            isAttacking = true;

            if (animator != null)
                animator.SetBool("isWalking", false);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void RotateAwayFromPlayer()
    {
        Vector2 direction = -(player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void AttackPlayer()
    {
        if (projectile == null || firePoint == null)
        {
            Debug.LogError("Projectile or FirePoint is missing.");
            return;
        }

        GameObject bullet = Instantiate(projectile, firePoint.position, firePoint.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetEnemyBullet(true);
        }

        if (animator != null)
        {
            animator.SetTrigger(attackTriggerName);
        }

        fireRateTimer = fireRate;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rb.linearVelocity = direction.normalized * force;

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }
}