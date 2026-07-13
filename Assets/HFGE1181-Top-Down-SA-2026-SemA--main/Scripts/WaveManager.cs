using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 5;
        public float spawnInterval = 0.5f;
    }

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;

    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;

    [Header("Timing")]
    [SerializeField] private float timeBetweenWaves = 10f;

    [HideInInspector] public UnityEvent<int> onWaveStarted;
    [HideInInspector] public UnityEvent onAllWavesComplete;
    [HideInInspector] public UnityEvent<float> onWaveTimerUpdated;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private int enemiesRemaining = 0;

    private bool waitingForWaveClear = false;
    private bool isWaveActive = false;

    private float waveTimer;

    private void Start()
    {
        StartCoroutine(WaveLoop());
    }

    private void Update()
    {
        if (waveTimer > 0f)
        {
            waveTimer -= Time.deltaTime;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateNextWaveTimer(waveTimer);
            }
        }
    }

    private IEnumerator WaveLoop()
    {
        while (++currentWaveIndex < waves.Count)
        {
            yield return StartCoroutine(IntermissionTimer(timeBetweenWaves));

            StartWave(currentWaveIndex);

            yield return StartCoroutine(SpawnEnemies(waves[currentWaveIndex]));
        }

        onAllWavesComplete?.Invoke();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private IEnumerator IntermissionTimer(float duration)
    {
        waveTimer = duration;

        while (waveTimer > 0f)
        {
            onWaveTimerUpdated?.Invoke(waveTimer);

            waveTimer -= Time.deltaTime;

            yield return null;
        }

        onWaveTimerUpdated?.Invoke(0f);
    }

    private void StartWave(int waveIndex)
    {
        isWaveActive = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCurrentWave(waveIndex + 1, waves.Count);
        }

        onWaveStarted?.Invoke(waveIndex);
    }

    private IEnumerator SpawnEnemies(Wave wave)
    {
        enemiesRemaining = wave.enemyCount;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnemiesRemaining(enemiesRemaining);
        }

        while (GetActiveSpawnPoints().Count == 0)
        {
            yield return null;
        }

        for (int i = 0; i < wave.enemyCount; i++)
        {
            List<Transform> activeSpawns = GetActiveSpawnPoints();

            if (activeSpawns.Count > 0)
            {
                Transform spawnPoint = activeSpawns[Random.Range(0, activeSpawns.Count)];

                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                enemiesAlive++;
            }

            yield return new WaitForSeconds(wave.spawnInterval);

            while (GetActiveSpawnPoints().Count == 0)
            {
                yield return null;
            }
        }

        waitingForWaveClear = true;

        while (enemiesAlive > 0)
        {
            yield return null;
        }

        waitingForWaveClear = false;
        isWaveActive = false;
    }

    private List<Transform> GetActiveSpawnPoints()
    {
        List<Transform> activeSpawns = new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.gameObject.activeInHierarchy)
            {
                activeSpawns.Add(spawnPoint);
            }
        }

        return activeSpawns;
    }

    public void EnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }
    }

    public void EnemyDied()
    {
        enemiesRemaining--;

        if (enemiesRemaining < 0)
        {
            enemiesRemaining = 0;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnemiesRemaining(enemiesRemaining);
        }
    }
}