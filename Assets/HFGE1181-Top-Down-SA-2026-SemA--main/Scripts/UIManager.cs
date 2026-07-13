using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    [Header("Wave UI")]
    public TMP_Text nextWaveTimerText;
    public TMP_Text currentWaveText;

    [Header("Enemy UI")]
    public TMP_Text enemiesRemainingText;

    [Header("Reload UI")]
    public Slider reloadSlider;

    [Header("Interact UI")]
    public TMP_Text interactText;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealth(float current, float max)
    {
        if (healthSlider != null) healthSlider.value = current / max;
        if (healthText != null) healthText.text = $"{Mathf.Ceil(current)} / {Mathf.Ceil(max)}";
    }

    public void UpdateNextWaveTimer(float time)
    {
        if (nextWaveTimerText != null) nextWaveTimerText.text = $"Next Wave: {time:0.0}s";
    }

    public void UpdateReloadProgress(float progress)
    {
        if (reloadSlider != null) reloadSlider.value = progress;
    }

    public void UpdateInteractText(string prompt)
    {
        if (interactText != null) interactText.text = prompt;
    }
    public void UpdateEnemiesRemaining(int remaining)
    {
        if (enemiesRemainingText != null)
            enemiesRemainingText.text = $"Enemies Remaining: {remaining}";
    }
    public void UpdateCurrentWave(int currentWave, int totalWaves)
    {
        if (currentWaveText != null)
        {
            currentWaveText.text = $"Wave {currentWave} / {totalWaves}";
        }
    }
}
