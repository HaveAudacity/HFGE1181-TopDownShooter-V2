using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float knockbackForce = 10f;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
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
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockback(knockbackDir, knockbackForce);
            }

            if (!isPiercing)
            {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("StopBox"))
        {
            Destroy(gameObject);
        }
    }
}
