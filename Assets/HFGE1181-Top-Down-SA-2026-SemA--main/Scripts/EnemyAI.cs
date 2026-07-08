using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float minimumPlayerDistance = 0.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float stunDurationAfterKnockback = 0.5f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private string attackTriggerName = "Attack";

    private Transform enemy;
    private Rigidbody2D rb;
    private Animator animator;
    private GameObject playerObj;

    private bool isKnockedBack = false;
    private bool isAttacking = false;
    private float fireRateTimer = 0f;
    private float knockbackTimer = 0f;
    private float stunTimer = 0f;
    private bool isStunned = false;

    private void Awake()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            enemy = playerObj.transform;
        }
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;

                isStunned = true;
                stunTimer = stunDurationAfterKnockback;
            }
            animator.SetBool("isWalking", false);
            return;
        }

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
            else
            {
                animator.SetBool("isWalking", false);
                return;
            }
        }

        if (isAttacking)
        {
            fireRateTimer -= Time.deltaTime;
            if (fireRateTimer <= 0f)
            {
                AttackPlayer();
            }

        }

        if (enemy != null)
        {
            if (CalculateDistance() > minimumPlayerDistance)
            {
                RotateTowardsPlayer();
            }
            else
            {
                RotateAwayFromPlayer();
            }

                Movement();

        }
    }

    private float CalculateDistance()
    {
        float distance = Vector2.Distance(transform.position, enemy.position);
        return distance;
    }

    private void Movement()
    {
        PlayerHealth pH = playerObj.GetComponent<PlayerHealth>();
        if (pH != null && pH.isPlayerDead)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        if (CalculateDistance() !<= attackRange)
        {
            isAttacking = false;
        }

        if (CalculateDistance() > attackRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                enemy.position,
                moveSpeed * Time.deltaTime
            );

            animator.SetBool("isWalking", true);
        }
        else if (CalculateDistance() <= attackRange && CalculateDistance() > minimumPlayerDistance)
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
        }
        else if (CalculateDistance() <= minimumPlayerDistance)
        {

            transform.position = Vector2.MoveTowards(
               transform.position,
               transform.position + (transform.position - enemy.position),
               moveSpeed * Time.deltaTime
           );

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = (enemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RotateAwayFromPlayer()
    {
        Vector2 direction = -1 * (enemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void AttackPlayer()
    {
        animator.SetTrigger(attackTriggerName);
        GameObject bullet = Instantiate(projectile, firePoint.position, firePoint.rotation);
        fireRateTimer = fireRate;

    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rb.linearVelocity = direction.normalized * force;
        animator.SetBool("isWalking", false);
    }
}
