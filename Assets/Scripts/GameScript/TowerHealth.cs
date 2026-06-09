using UnityEngine;
using UnityEngine.UI;
using TMPro; // REQUIRED to control TextMeshPro text elements!

public class TowerHealth : MonoBehaviour
{
    [Header("Tower Health Settings")]
    [Tooltip("The maximum health capacity of this tower. Can be modified safely.")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI Reference Elements")]
    [Tooltip("Drag your TowerHPBar Slider component here.")]
    public Slider hpSlider;

    [Tooltip("Drag your 'TowerHP_txt' TextMeshPro component here.")]
    public TextMeshProUGUI hpText;

    [Header("Outside Script Connections")]
    public winLoseScript winLosePanel; // Reference to the win/lose panel script

    void Start()
    {
        currentHealth = maxHealth;
        
        // Push the starting health values out to the UI canvas layout
        UpdateTowerHPUI();
    }

    /// <summary>
    /// Deducts custom health from the tower and instantly refreshes the visual UI.
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Keep health values bound between 0 and your maxHealth variable limit
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateTowerHPUI();

        if (currentHealth <= 0f)
        {
            DestroyTower();
        }
    }

    /// <summary>
    /// Updates both the slider filler amount and the string text layout simultaneously.
    /// </summary>
    public void UpdateTowerHPUI()
    {
        // 1. Manage Slider Bar Progression Fill
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth / maxHealth;
        }

        // 2. Manage Real-Time Text String Formatting (e.g., "450 / 500 HP")
        if (hpText != null)
        {
            // Mathf.RoundToInt keeps the display clean without floating point decimals
            hpText.text = Mathf.RoundToInt(currentHealth) + " / " + Mathf.RoundToInt(maxHealth) + " HP";
        }
    }

    private void DestroyTower()
	{
		Debug.Log(gameObject.name + " has been destroyed!");

        // If the destroyed tower is an Enemy, the player wins. Otherwise, the player loses.
        bool isPlayerVictory = gameObject.CompareTag("Enemy");
        string gameResultSignal = "";
        
        if (isPlayerVictory)
        {
            gameResultSignal = "Win";
            winLosePanel.WinLose(gameResultSignal, "Congratulations! You have won the game!");
            Debug.Log("Signal set: Player Won! Loading Ending Scene...");
        }
        else
        {
            gameResultSignal = "Lose";
            winLosePanel.WinLose(gameResultSignal, "Sorry, you have lost the game.");
            Debug.Log("Signal set: Player Lost! Loading Ending Scene...");
        }

		Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        characterAttackScript collidingUnit = collision.GetComponent<characterAttackScript>();

        if (collidingUnit != null)
        {
            if (!collidingUnit.gameObject.CompareTag(this.gameObject.tag))
            {
                int unitRemainingHP = collidingUnit.GetCurrentHP();
                TakeDamage(unitRemainingHP);

                Debug.Log(collision.gameObject.name + " crashed into " + gameObject.name + " dealing " + unitRemainingHP + " damage!");
                Destroy(collidingUnit.gameObject);
            }
        }
    }
}