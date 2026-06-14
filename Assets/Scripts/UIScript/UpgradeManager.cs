using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("Connections")]
    public moneyExpScript moneyScript; // Still uses this script since it handles both money and EXP variables

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
    }

    // --- BUTTON TRIGGER FUNCTIONS ---

    public void UpgradeUnitHP()
    {
        int currentCost = CalculateCost(hpUpgradeLevel);
        // FIX: Check current EXP points instead of money balances
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            hpUpgradeLevel++;
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
        // FIX: Check current EXP points instead of money balances
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            damageUpgradeLevel++;
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
        // FIX: Check current EXP points instead of money balances
        if (moneyScript != null && moneyScript.GetCurrentExp() >= currentCost)
        {
            moneyScript.DeductExp(currentCost); // Deduct EXP
            deploymentUpgradeLevel++;
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
}