using UnityEngine;

public class enemySpawnScript : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drag the enemy Prefab here.")]
    public GameObject enemyPrefab;

    [Tooltip("Drag the GameObject representing the spawn location here.")]
    public Transform spawnPoint;

    [Tooltip("Drag the GameObject that will hold all spawned enemies here.")]
    public Transform parentGroup;

    [Header("Time Settings")]
    [Tooltip("Time in seconds between each spawn.")]
    public float spawnInterval = 3f;

    [Header("Unit Cap Settings")]
    public int maxEnemyUnits = 12;

    private float timer;

    void Start()
    {
        timer = 0f; 
    }

    void Update()
    {
        // Check how many enemies are currently active on the field
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        // --- PAUSE SPAWNING UNIT MECHANIC ---
        // If enemy army has hit 12 units, pause completely and do not advance the stopwatch timer
        if (currentEnemyCount >= maxEnemyUnits)
        {
            return; 
        }

        // Only count up the interval time if there is an available unit slot
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; 
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            if (parentGroup != null)
            {
                newEnemy.transform.SetParent(parentGroup);
            }
        }
        else
        {
            Debug.LogWarning("Cannot spawn: Enemy Prefab or Spawn Point is missing in the Inspector!");
        }
    }
}