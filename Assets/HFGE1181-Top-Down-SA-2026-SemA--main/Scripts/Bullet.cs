using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float knockbackForce = 10f;

    [Header("Who fired this bullet?")]
    [SerializeField] private bool enemyBullet = false;

    private bool isPiercing = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public void SetPiercing(bool value)
    {
        isPiercing = value;
    }

    public void SetEnemyBullet(bool value)
    {
        enemyBullet = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyBullet)
        {
            if (other.CompareTag("Enemy"))
                return;

            if (other.CompareTag("Player"))
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();

                if (player != null)
                {
                    player.TakeDamage(damage, transform.position);
                }

                Destroy(gameObject);
            }

            return;
        }

        // PLAYER BULLET

        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            if (enemyAI != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockback(dir, knockbackForce);
            }

            if (!isPiercing)
            {
                Destroy(gameObject);
            }

            return;
        }

        if (other.CompareTag("StopBox"))
        {
            Destroy(gameObject);
        }
    }
}