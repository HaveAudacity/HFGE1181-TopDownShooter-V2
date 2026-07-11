using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private bool canDie = true;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 2.0f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    [HideInInspector] public UnityEvent onDeath;
    [HideInInspector] public UnityEvent<int, int> onHealthChanged;

    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Enemy took " + amount + " damage");
        if (amount <= 0 || currentHealth <= 0 || isDead)
            return;

        currentHealth -= amount;
        Debug.Log("Current Health = " + currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("EnemyTakeDamage");
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Health reached zero");
            Debug.Log("canDie = " + canDie);

            if (canDie)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died");

        if (isDead)
        {
            return;
        }

        isDead = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("EnemyDeath");
        }

        onDeath?.Invoke();

        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
