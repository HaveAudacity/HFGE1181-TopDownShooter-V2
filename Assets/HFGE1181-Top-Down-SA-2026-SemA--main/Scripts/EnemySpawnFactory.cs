using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnFactory : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public int enemyCount = 5;
        public float spawnInterval = 0.5f;
    }

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();

    [Header("Spawning")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Movement Parameters")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float speed;
    [SerializeField] private float distanceAllowance;

    [HideInInspector] public UnityEvent<int> onWaveStarted;
    [HideInInspector] public UnityEvent onAllWavesComplete;
    [HideInInspector] public UnityEvent<float> onWaveTimerUpdated;

    private int patrolIndex = 0;
    private int currentWaveIndex = -1;
    private float timeBetweenWaves;
    private float waveTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            UIManager.Instance.UpdateNextWaveTimer(waveTimer);
        }

        FactoryMovement();
    }

    private void FactoryMovement()
    {
        
        float distance = Vector3.Distance(transform.position, patrolPoints[patrolIndex].position);

        if (distance >= distanceAllowance)
        {
            transform.position = Vector3.MoveTowards(transform.position, patrolPoints[patrolIndex].position, speed * Time.deltaTime);
        }
        else
        {
            if (patrolIndex < patrolPoints.Length - 1) 
            {

                patrolIndex++;
                
            }
            else if (patrolIndex ==  patrolPoints.Length - 1)
            {
                patrolIndex = 0;
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
        onWaveStarted?.Invoke(waveIndex);
        Debug.Log($"Wave {waveIndex + 1} started!");
    }

    private IEnumerator SpawnEnemies(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

}
