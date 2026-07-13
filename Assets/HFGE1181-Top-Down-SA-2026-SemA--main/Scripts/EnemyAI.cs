using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

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

        if (playerObj == null)
        {
            Debug.LogError("Player NOT found!");
        }
        else
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

            if (animator != null)
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
                if (animator != null)
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
        return Vector2.Distance(transform.position, enemy.position);
    }

    private void Movement()
    {
        PlayerHealth pH = playerObj.GetComponent<PlayerHealth>();

       
        if (pH != null && pH.isPlayerDead)
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
                enemy.position,
                moveSpeed * Time.deltaTime
            );

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
        Vector2 direction = (enemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RotateAwayFromPlayer()
    {
        Vector2 direction = -(enemy.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
   

    private void AttackPlayer()
    {
        if (projectile == null || firePoint == null)
        {
            Debug.LogError("Projectile or FirePoint is missing!");
            return;
        }
       
        Debug.Log("Enemy Fired");

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