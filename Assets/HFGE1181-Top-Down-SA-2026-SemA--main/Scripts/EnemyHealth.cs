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
      ;
        if (amount <= 0 || currentHealth <= 0 || isDead)
            return;

        currentHealth -= amount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("EnemyTakeDamage");
        }

        if (currentHealth <= 0)
        {
            

            if (canDie)
            {
                Die();
            }
        }
    }

    private void Die()
    {
       

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
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();

        if (waveManager != null)
        {
            waveManager.EnemyKilled();
        }

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
