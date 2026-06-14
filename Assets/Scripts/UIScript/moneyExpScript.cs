using UnityEngine;
using TMPro; // Required to communicate with TextMeshPro

public class moneyExpScript : MonoBehaviour
{
    [Header("Initial Starting Amounts")]
    public int initialMoney = 0; 
    public int initialExp = 0;

    [Header("Inspector Default Rewards")]
    public int defaultMoneyReward = 10;
    public int defaultExpReward = 25;

    [Header("Periodic Money Settings")]
    // Check this box in the Inspector to turn on automatic money
    public bool enablePeriodicMoney = true;
    // How many seconds between each payout?
    public float moneyInterval = 5f;
    // How much money do you get every time the interval passes?
    public int periodicMoneyAmount = 5;
    // Internal timer to track when the next payout should happen
    private float nextMoneyTime = 0f;

    [Header("Canvas Connections")]
    public TextMeshProUGUI moneyText; 
    public TextMeshProUGUI expText;

    private int currentMoney;
    private int currentExp;

    void Start()
    {
        currentMoney = initialMoney;
        currentExp = initialExp;
        
        // Start the timer as soon as the game begins
        nextMoneyTime = Time.time + moneyInterval;

        UpdateUI();
    }

    void Update()
    {
        // This checks two things every single frame:
        // 1. Is the automatic money box checked?
        // 2. Has enough time passed since the last payout?
        if (enablePeriodicMoney && Time.time >= nextMoneyTime)
        {
            // Add the money
            currentMoney += periodicMoneyAmount;
            
            // Update the Canvas
            UpdateUI();
            
            // Reset the stopwatch so we have to wait another 5 seconds
            nextMoneyTime = Time.time + moneyInterval;
        }
    }

    // --- MANUALLY ADDING MONEY FUNCTIONS ---

    public void AddDefaultMoney()
    {
        currentMoney += defaultMoneyReward;
        UpdateUI();
    }

    public void AddMoney(int amountToAdd)
    {
        currentMoney += amountToAdd;
        UpdateUI();
    }

    // --- EXP FUNCTIONS ---

    public void AddExp(int amountToAdd)
    {
        currentExp += amountToAdd;
        UpdateUI();
    }

    public void AddDefaultExp()
    {
        currentExp += defaultExpReward;
        UpdateUI();
    }
	

	public int GetCurrentMoney()
	{
		return currentMoney;
	}

	public void DeductMoney(int amountToSubtract)
	{
		currentMoney -= amountToSubtract;
		
		// Prevent money from accidentally dropping below 0
		if (currentMoney < 0) 
		{
			currentMoney = 0;
		}
		
		UpdateUI(); // Refresh the screen text instantly!
	}
	
    // --- UI UPDATING FUNCTION ---
    
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + currentMoney.ToString(); 
        }
        
        if (expText != null)
        {
            expText.text = "EXP: " + currentExp.ToString();
        }
    }

	// --- NEW: EXP GETTER AND DEDUCTION FUNCTIONS FOR UPGRADES ---

    /// <summary>
    /// Public getter so the UpgradeManager can check how much EXP the player has accumulated.
    /// </summary>
    public int GetCurrentExp()
    {
        return currentExp;
    }

    /// <summary>
    /// Spends/deducts a specified amount of EXP from the player profile and refreshes the canvas UI text.
    /// </summary>
    public void DeductExp(int amountToSubtract)
    {
        currentExp -= amountToSubtract;
        
        // Prevent EXP from accidentally dropping below zero
        if (currentExp < 0) 
        {
            currentExp = 0;
        }
        
        UpdateUI(); // Refresh the screen text canvas instantly!
    }

}