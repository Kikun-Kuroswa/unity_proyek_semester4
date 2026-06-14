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

    [Header("Audio SFX Settings")]
    [Tooltip("The audio sound file that plays when successfully clearing the screen.")]
    public AudioClip destroyChildrenSFX;
    [Tooltip("The audio sound file that plays when successfully summoning the assassin squad.")]
    public AudioClip spawnAssassinSFX;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    /// <summary>
    /// Destroys all immediate child GameObjects attached to the targeted group object.
    /// </summary>
    public void DestroyAllChildren()
    {
        // 1. CHOOSE IF PLAYER PASSES THE CHECK
        if(moneyExpManager.GetCurrentExp() < 250)
        {
            Debug.Log("Not enough EXP to use this ability! You need at least 250 EXP.");
            return; // Exit early
        }

        // 2. PLAY SFX ONLY AFTER PASSING THE CHECK ABOVE
        if (destroyChildrenSFX != null)
        {
            // Using Main Camera position ensures the 3D world sound is perfectly audible to the player
            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(destroyChildrenSFX, soundPosition, sfxVolume);
        }

        moneyExpManager.DeductExp(250); // Deduct 250 EXP

        // Fix: Use targetParent consistently
        Transform targetParent = enemyGroup != null ? enemyGroup : transform;

        // FIXED: Loop through 'targetParent' instead of 'enemyGroup' to avoid NullReferenceExceptions
        foreach (Transform child in targetParent) 
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log($"All children of {targetParent.name} have been destroyed!");
    }

    public void SpawnAlotOfAssasin()
    {
        // 1. CHOOSE IF PLAYER PASSES THE CHECK
        if(moneyExpManager.GetCurrentExp() < 125)
        {
            Debug.Log("Not enough EXP to use this ability! You need at least 125 EXP.");
            return; // Exit early
        }

        // 2. PLAY SFX ONLY AFTER PASSING THE CHECK ABOVE
        if (spawnAssassinSFX != null)
        {
            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioSource.PlayClipAtPoint(spawnAssassinSFX, soundPosition, sfxVolume);
        }

        moneyExpManager.DeductExp(125); // Deduct 125 EXP

        Transform targetParent = allyGroup != null ? allyGroup : transform;

        for (int i = 0; i < 5; i++)
        {
            GameObject newAlly = Instantiate(allyPrefab, allySpawnPoint.position, allySpawnPoint.rotation);
            newAlly.transform.SetParent(targetParent);
        }
        
        Debug.Log("Spawned 5 assassins successfully!");
    }
}