using UnityEngine;

public class Blockade : MonoBehaviour
{
    [Header("Barricade Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Enemy Damage")]
    [SerializeField] private float damagePerSecond = 20f;

    private float health;


    private void Start()
    {
        health = maxHealth;
    }


    private void Update()
    {
        if (health <= 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play("BarricadeBreak");
            }

            Destroy(gameObject);
        }
    }


    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log("Barricade Health: " + health);
    }
}