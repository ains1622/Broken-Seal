using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // A list of groups of enemies in this wave
        public int waveQuota; // The total number of enemies in this wave
        public float spawnInterval; // The interval at wich to spawn enemies
        public int spawnCount; // The number of enemies already spawned in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // The number of enemies to spawn in this wave
        public int spawnCount; // The number of enemies of this type already spawned in this wave
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; // A list of all the waves in the game
    public int currentWaveCount; // The index of the current wave [Remember, a list starts at 0]

    [Header("Spawner Attributes")]
    float spawnTimer; // Timer use to determine when to spawn the next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed; // The maximum number of enemies allowed on the map
    public bool maxEnemiesReached = false; // A flag indicating if the maximum number of enemies has been reached
    public float waveInterval; // The interval between each wave
    bool isWaveActive = false; // A flag indicating if the current wave is active

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPositions; // A list to store all the relative spawn points of enemies

    Transform player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive) // Check if the wave has ended and the next wave should start
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn the next enemy
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f; // Reset the timer after spawning
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        // Wave for 'waveInterval' seconds before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        // If there are more waves to start after the current wave, move to the next wave
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        // Calculate the total number of enemies in the current wave
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
    }

    /// <summary>
    /// This method will stop spawning enemies if the amount of enemies in the map is maximum
    /// The method will only spawn enemies in a particular until is time for the next wave's enemies to be spawned
    /// <summary>
        void SpawnEnemies()
    {
        // Check if the minimum number of enemies in the wave have been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn each type of enemy until the quota is filled
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check if the minimum number of enemies of this type have been spawned
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    Vector3 spawnOffset = relativeSpawnPositions[Random.Range(0, relativeSpawnPositions.Count)].position;
                    Vector3 spawnPosition = player.position + spawnOffset;

                    // Instanciar enemigo
                    GameObject enemy = Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    // Si es jefe, reposicionar y hacer intro
                    BossController boss = enemy.GetComponent<BossController>();
                    if (boss != null)
                    {
                        Vector3 bossSpawnPosition = player.position + Vector3.up * 5f;
                        enemy.transform.position = bossSpawnPosition;

                        boss.InitiateIntroAttack(player.position);
                    }

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    // Call this function when an enemy is killed
    public void OnEnemyKilled()
    {
        // Decrement the number of enemies alive
        enemiesAlive--;
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
