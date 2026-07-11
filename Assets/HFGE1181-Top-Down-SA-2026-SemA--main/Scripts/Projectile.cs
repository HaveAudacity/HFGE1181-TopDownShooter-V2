using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the player
        if (other.CompareTag("Player"))
        {
            return;
        }

        // Damage enemies
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // Destroy on any valid collision except the player
        Destroy(gameObject);
    }
}



