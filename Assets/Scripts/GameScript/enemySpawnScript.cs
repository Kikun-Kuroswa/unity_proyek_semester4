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

    // This internal timer keeps track of how much time has passed
    private float timer;

    void Start()
    {
        // We start the timer at 0 so it begins counting up immediately when the game starts
        timer = 0f; 
    }

    void Update()
    {
        // Time.deltaTime adds the exact time that has passed since the last frame
        timer += Time.deltaTime;

        // Check if the timer has reached or exceeded your set interval
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            
            // Reset the timer back to 0 for the next spawn cycle
            timer = 0f; 
        }
    }

    void SpawnEnemy()
    {
        // A safety check to ensure you actually assigned an enemy in the Inspector
        if (enemyPrefab != null && spawnPoint != null)
        {
            // Instantiate creates a clone of your enemy at the spawn point's exact position and rotation
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // If you assigned a parent group, put the newly spawned enemy inside it
            if (parentGroup != null)
            {
                newEnemy.transform.SetParent(parentGroup);
            }
        }
        else
        {
            // If you forgot to assign the objects in the Inspector, this prints a helpful error in the Console
            Debug.LogWarning("Cannot spawn: Enemy Prefab or Spawn Point is missing in the Inspector!");
        }
    }
}