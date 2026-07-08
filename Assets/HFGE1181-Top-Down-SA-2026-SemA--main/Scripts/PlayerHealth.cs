using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 5.0f;

    [Header("Damage Response")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    [HideInInspector] public UnityEvent onDeath;
    [HideInInspector] public UnityEvent<int, int> onHealthChanged;
    public bool isPlayerDead = false;

    private int currentHealth;
    private bool isInvincible = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount, Vector2 hitSource)
    {
        if (amount <= 0 || currentHealth <= 0 || isInvincible)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        UIManager.Instance.UpdateHealth(currentHealth, maxHealth);

        AudioManager.Instance.Play("PlayerTakeDamage");

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector2 knockbackDir = ((Vector2)rb.position - hitSource).normalized;
            playerController.ApplyKnockback(knockbackDir);
        }

        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        onDeath?.Invoke();

        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        gameObject.tag = "Untagged";
        isPlayerDead = true;

        AudioManager.Instance.Play("PlayerDeath");

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(invincibilityDuration);
        spriteRenderer.color = originalColor;
        isInvincible = false;
    }
}
