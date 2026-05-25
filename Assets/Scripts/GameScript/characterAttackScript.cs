using UnityEngine;
using TMPro; // This line is required to talk to TextMeshPro!

public class characterAttackScript : MonoBehaviour
{
    [Header("Health Settings")]
    // Set this in the inspector to whatever you want (e.g., 100)
    public int maxHP = 100; 
    private int currentHP;
    
    // Drag your Canvas TextMeshPro object into this slot in the inspector
    public TextMeshProUGUI hpText; 

    [Header("Attack Settings")]
    public int attackDamage = 10;
    
    // The delay between attacks in seconds
    public float attackInterval = 1.5f;

    [Header("Exp Settings")]
    public int expReward = 25;
    public bool givesExpWhenDead = true; // Check this box in the inspector if this character should give EXP when defeated


    [Header("Outside Script Connections")]
    public moneyExpScript moneyExpScript; // Reference to the money/exp script
    
    // An internal timer to track when we are allowed to attack again
    private float nextAttackTime = 0f;

    void Start()
    {
        // When the game starts, set current HP to your max HP
        currentHP = maxHP;
        
        // Immediately update the canvas to show the starting HP
        UpdateHPDisplay(); 
    }

    // --- YOUR CANVAS FUNCTION ---
    // This updates the TextMeshPro on your character to display the current HP
    public void UpdateHPDisplay()
    {
        // Safety check to ensure you actually linked a text object in the inspector
        if (hpText != null)
        {
            // Changes the text box to strictly display the number
            hpText.text = currentHP.ToString(); 
        }
    }

    // --- TAKING DAMAGE FUNCTION ---
    // Other scripts (or enemies with this exact script) will call this to hurt this character
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;

        // Prevent HP from dropping into negative numbers
        if (currentHP < 0) 
        {
            currentHP = 0;
        }

        // Instantly update the Canvas whenever health is lost
        UpdateHPDisplay();

        if (currentHP == 0)
        {
            if (givesExpWhenDead)
            {
                moneyExpScript.AddExp(expReward); // Give the player EXP for defeating this character
            }
            Destroy(gameObject); // This will remove the character from the game when HP hits 0
            Debug.Log(gameObject.name + " has 0 HP!");
            // You can put death logic here later, like: Destroy(gameObject);
        }
    }

    // --- ATTACKING FUNCTION ---
    // This uses your Trigger Collider. Whenever an enemy stays inside your attack hitbox, 
    // it will try to attack them on a repeating timer.
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if enough time has passed based on your interval
        if (Time.time >= nextAttackTime)
        {
            // Look at the object in our hitbox. Does it have this script so it can take damage?
            characterAttackScript target = collision.GetComponent<characterAttackScript>();

            // If it DOES have the script (meaning it has health and can take damage)
            if (target != null)
            {
                // --- NEW CODE: THE FRIENDLY FIRE CHECK ---
                // We compare the sticky note Tag of the target with our own sticky note Tag.
                // The "!" symbol means "NOT". So we only attack if the tags do NOT match!
                if (!target.gameObject.CompareTag(this.gameObject.tag))
                {
                    // Deal damage using the function we made above
                    target.TakeDamage(attackDamage);

                    // Reset our attack timer so we have to wait for the interval again
                    nextAttackTime = Time.time + attackInterval;
                }
            }
        }
    }
}