using UnityEngine;

public class Blockade : MonoBehaviour
{
    [Header("Barricade Health")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            BreakBarricade();
        }
    }

    private void BreakBarricade()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("BarricadeBreak");
        }

        Destroy(gameObject);
    }
}