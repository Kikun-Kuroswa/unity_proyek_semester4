using UnityEngine;

public class soldierSpawnScript : MonoBehaviour
{
    [Header("Soldier Prefabs")]
    [Tooltip("Assign the first type of soldier prefab here.")]
    public GameObject soldierType1Prefab;

    [Tooltip("Assign the second type of soldier prefab here.")]
    public GameObject soldierType2Prefab;

    [Header("Placement & Organization")]
    [Tooltip("Where in the game world should they physically spawn? Drag a Transform here.")]
    public Transform spawnLocation;

    [Tooltip("Where in the Hierarchy should they be grouped? Drag an empty GameObject here.")]
    public Transform hierarchyGroupParent;

    /// <summary>
    /// Call this function from your first UI Button.
    /// </summary>
    public void SpawnSoldierType1()
    {
        Spawn(soldierType1Prefab);
    }

    /// <summary>
    /// Call this function from your second UI Button.
    /// </summary>
    public void SpawnSoldierType2()
    {
        Spawn(soldierType2Prefab);
    }

    /// <summary>
    /// The core logic that handles the actual creation and grouping.
    /// </summary>
    private void Spawn(GameObject prefabToSpawn)
    {
        // 1. Safety Checks
        if (prefabToSpawn == null)
        {
            Debug.LogError("A Soldier Prefab is missing! Please assign it in the Inspector.");
            return;
        }
        if (spawnLocation == null)
        {
            Debug.LogError("Spawn Location is missing! Please assign a Transform in the Inspector.");
            return;
        }

        // 2. Physical Spawning (using the dedicated spawnLocation, NOT the script's location)
        GameObject newSoldier = Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);

        // 3. Hierarchy Grouping
        if (hierarchyGroupParent != null)
        {
            newSoldier.transform.SetParent(hierarchyGroupParent);
        }
        else
        {
            Debug.LogWarning("Hierarchy Group Parent is not assigned. Soldiers will clutter the main list.");
        }
    }
}