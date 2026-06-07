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
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // NEW CONDITIONAL: Find how many active clones are currently in the level
            // Note: Ensure your Enemy Prefab is given a unique Tag like "Enemy" in the Inspector,
            // or use "Untagged" if that is what they currently use.
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemyCount < 12)
            {
                SpawnEnemy();
            }
            else
            {
                Debug.Log("Enemy deployment skipped: Max limit of 12 units reached on battlefield.");
            }
            
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