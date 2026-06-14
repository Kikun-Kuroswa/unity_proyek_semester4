using UnityEngine;

public class specialAbilityScript : MonoBehaviour
{
    [Tooltip("Drag the group object (parent) here in the Unity Inspector.")]
    public Transform enemyGroup; // The parent object under which all enemy units are organized

    [Header("Exp Script")]
    public moneyExpScript moneyExpManager; // Reference to the moneyExpScript to access its functions
    [Header("Assassin Spawn Settings")]
    public Transform allyGroup; // The parent object under which all assassin units are organized
    public GameObject allyPrefab; // The prefab for the assassin unit to be spawned
    public Transform allySpawnPoint; // The point where assassin units will be spawned
    void Start()
    {
        // Initialization code here if needed
    }

    void Update()
    {

    }

    /// <summary>
    /// Destroys all immediate child GameObjects attached to the targeted group object.
    /// </summary>
    public void DestroyAllChildren()
    {
        if(moneyExpManager.GetCurrentExp() < 250)
        {
            Debug.Log("Not enough EXP to use this ability! You need at least 250 EXP.");
            return; // Exit the function early if the player doesn't have enough EXP
        }

        moneyExpManager.DeductExp(250); // Deduct 250 EXP from the player's total

        // If no enemyGroup is assigned in the Inspector, fallback to the object this script is attached to
        Transform targetParent = enemyGroup != null ? enemyGroup : transform;

        // Loop through all children of the target parent
        foreach (Transform child in enemyGroup)
        {
            // Destroy the GameObject the child Transform is attached to
            Destroy(child.gameObject);
        }
        
        Debug.Log($"All children of {enemyGroup.name} have been destroyed!");
    }

    public void SpawnAlotOfAssasin()
    {
        if(moneyExpManager.GetCurrentExp() < 125)
        {
            Debug.Log("Not enough EXP to use this ability! You need at least 125 EXP.");
            return; // Exit the function early if the player doesn't have enough EXP
        }

        moneyExpManager.DeductExp(125); // Deduct 125 EXP from the player's total

        // If no allyGroup is assigned in the Inspector, fallback to the object this script is attached to
        Transform targetParent = allyGroup != null ? allyGroup : transform;

        for (int i = 0; i < 5; i++)
        {
            // Create a new GameObject for the assassin
            GameObject newAlly = Instantiate(allyPrefab, allySpawnPoint.position, allySpawnPoint.rotation);
            // Set the assassin's parent to the target parent
            newAlly.transform.SetParent(targetParent);
            // Optionally, you can set the position, add components, etc. here
        }
        
        Debug.Log("Spawned 5 assassins under " + targetParent.name);
    }
}