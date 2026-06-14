using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Connections")]
    public moneyExpScript moneyScript; // Still uses this script since it handles both money and EXP variables

    [Header("Audio Settings")]
    [Tooltip("Drag your AudioSource component here")]
    public AudioSource audioSource;
    [Tooltip("Drag your upgrade success SFX clip here")]
    public AudioClip upgradeSuccessSFX;

    [Header("Upgrade Levels")]
    public int hpUpgradeLevel = 0;
    public int damageUpgradeLevel = 0;
    public int deploymentUpgradeLevel = 0;

    [Header("Upgrade Multipliers / Increments")]
    [Tooltip("How much max HP increases per level (e.g., +20 HP per upgrade)")]
    public int hpIncrementPerLevel = 20;
    
    [Tooltip("How much damage increases per level (e.g., +5 damage per upgrade)")]
    public float damageIncrementPerLevel = 5f;

    [Tooltip("How many seconds are shaved off training times per level (e.g., 0.15 seconds faster)")]
    public float deploymentTimeReductionPerLevel = 0.15f;

    [Header("Upgrade Costs (In EXP)")]
    public int baseUpgradeCost = 50;
    public int costIncreasePerLevel = 25;

    // A global reference so any spawned unit or spawner can easily find this data instantly
    public static UpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (moneyScript == null)
        {
            moneyScript = Object.FindAnyObjectByType<moneyExpScript>();
        }

        // Optional fallback: If you forgot to assign the AudioSource, try to grab one on the same GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // --- BUTTON TRIGGER FUNCTIONS ---

    public void UpgradeUnitHP()
    {
        int currentCost = CalculateCost(hpUpgradeLevel);
        // CHECK: Does the player have enough EXP?
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            hpUpgradeLevel++;
            
            PlayUpgradeSFX(); // <-- PLAY SFX HERE (Only on success)

            Debug.Log($"HP Upgraded using EXP! Current Level: {hpUpgradeLevel}. Extra HP: +{hpUpgradeLevel * hpIncrementPerLevel}");
        }
        else
        {
            Debug.LogWarning("Not enough EXP to upgrade HP!");
        }
    }

    public void UpgradeUnitDamage()
    {
        int currentCost = CalculateCost(damageUpgradeLevel);
        // CHECK: Does the player have enough EXP?
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            damageUpgradeLevel++;

            PlayUpgradeSFX(); // <-- PLAY SFX HERE (Only on success)

            Debug.Log($"Damage Upgraded using EXP! Current Level: {damageUpgradeLevel}. Extra Damage: +{damageUpgradeLevel * damageIncrementPerLevel}");
        }
        else
        {
            Debug.LogWarning("Not enough EXP to upgrade Damage!");
        }
    }

    public void UpgradeDeploymentSpeed()
    {
        int currentCost = CalculateCost(deploymentUpgradeLevel);
        // CHECK: Does the player have enough EXP?
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            deploymentUpgradeLevel++;

            PlayUpgradeSFX(); // <-- PLAY SFX HERE (Only on success)

            Debug.Log($"Deployment Speed Upgraded using EXP! Current Level: {deploymentUpgradeLevel}. Training time reduced by: {deploymentUpgradeLevel * deploymentTimeReductionPerLevel}s");
        }
        else
        {
            Debug.LogWarning("Not enough EXP to upgrade Deployment Speed!");
        }
    }

    private int CalculateCost(int currentLevel)
    {
        return baseUpgradeCost + (currentLevel * costIncreasePerLevel);
    }

    // Helper method to safely play the sound effect without interrupting itself
    private void PlayUpgradeSFX()
    {
        if (audioSource != null && upgradeSuccessSFX != null)
        {
            // PlayOneShot allows overlapping sounds if the player clicks upgrades quickly
            audioSource.PlayOneShot(upgradeSuccessSFX);
        }
    }
}