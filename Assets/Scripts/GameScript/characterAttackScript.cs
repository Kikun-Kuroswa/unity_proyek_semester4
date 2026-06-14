using UnityEngine;
using TMPro; // REQUIRED to control TextMeshPro text elements![cite: 19]

public class characterAttackScript : MonoBehaviour
{
    [Header("Base Combat Stats")]
    public int maxHP = 100; //[cite: 19]
    [SerializeField] private int currentHP; //[cite: 19]
    public float attackDamage = 10f; //[cite: 19]
    public float attackInterval = 1.5f; //[cite: 19]
    protected float nextAttackTime; //[cite: 19]

    [Header("UI Text Reference")]
    [Tooltip("Drag your floating Canvas TextMeshPro component here.")]
    public TextMeshProUGUI hpText;  //[cite: 19]

    [Header("Ranged Specialist Settings")]
    public bool isRangedUnit = false; //[cite: 19]
    [Tooltip("The visual bullet prefab that will fly towards targets.")]
    public GameObject projectilePrefab; //[cite: 19]
    [Tooltip("The point relative to the character where the bullet spawns (e.g. hands or weapon tip).")]
    public Transform firePoint; //[cite: 19]
    public float attackRange = 5f; //[cite: 19]
    public float projectileSpeed = 8f; //[cite: 19]

    [Header("Audio SFX Settings")]
    [Tooltip("The audio sound file that plays when this unit swings or shoots.")]
    public AudioClip attackSFX; 
    [Range(0f, 1f)]
    [Tooltip("Volume of the attack sound effect.")]
    public float sfxVolume = 0.6f;

    [Header("Target Layer Filtering")]
    [Tooltip("Include the layers that units and towers occupy (e.g., Default, UI, or custom layers).")]
    public LayerMask targetLayers; //[cite: 19]

    [Header("Exp Settings")]
    public int expReward = 25; //[cite: 19]
    public bool givesExpWhenDead = true;  //[cite: 19]

    [Header("Outside Script Connections")]
    public moneyExpScript moneyExpScript;  //[cite: 19]

    private string opponentTag; //[cite: 19]
    private movementScript movementComp; //[cite: 19]
    private bool hasTargetInRange = false; //[cite: 19]

    void Start()
	{
		// Determine who our enemies are based on our own Tag assignment
		opponentTag = gameObject.CompareTag("soldier") ? "Enemy" : "soldier"; //[cite: 19]

		// --- FIX: ONLY APPLY UPGRADES IF THIS GAME OBJECT IS A PLAYER SOLDIER ---
		if (gameObject.CompareTag("soldier") && UpgradeManager.Instance != null) //[cite: 19]
		{
			// 1. Calculate and add upgraded HP to default base stats
			maxHP += UpgradeManager.Instance.hpUpgradeLevel * UpgradeManager.Instance.hpIncrementPerLevel; //[cite: 19]
			
			// 2. Calculate and add upgraded Damage
			attackDamage += UpgradeManager.Instance.damageUpgradeLevel * UpgradeManager.Instance.damageIncrementPerLevel; //[cite: 19]
		}

		// Set current HP to max HP (either base or upgraded) when spawning
		currentHP = maxHP; //[cite: 19]
		
		// Force text component to change from template strings to actual numbers!
		UpdateHPDisplay(); //[cite: 19]

		movementComp = GetComponent<movementScript>(); //[cite: 19]
		moneyExpScript = Object.FindAnyObjectByType<moneyExpScript>(); //[cite: 19]
		
		if (moneyExpScript == null) //[cite: 19]
		{
			Debug.LogWarning(gameObject.name + " spawned, but couldn't find a moneyExpScript in the scene!"); //[cite: 19]
		}
	}

    void Update()
    {
        if (isRangedUnit) //[cite: 19]
        {
            HandleRangedCombat(); //[cite: 19]
        }
    }

    private void HandleRangedCombat()
    {
        // Scan the surrounding space in a radius matching the attack range
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayers); //[cite: 19]
        Transform closestTarget = null; //[cite: 19]
        float closestDistance = Mathf.Infinity; //[cite: 19]

        foreach (Collider2D col in hitTargets) //[cite: 19]
        {
            // Verify if the scanned object belongs to the enemy side
            if (col.CompareTag(opponentTag)) //[cite: 19]
            {
                float distance = Vector2.Distance(transform.position, col.transform.position); //[cite: 19]
                if (distance < closestDistance) //[cite: 19]
                {
                    closestDistance = distance; //[cite: 19]
                    closestTarget = col.transform; //[cite: 19]
                }
            }
        }

        if (closestTarget != null) //[cite: 19]
        {
            hasTargetInRange = true; //[cite: 19]

            // Signal the movement script to halt walking behavior
            if (movementComp != null) movementComp.SetRangedCombatHalt(true); //[cite: 19]

            // Execute attack loop when ready
            if (Time.time >= nextAttackTime) //[cite: 19]
            {
                FireProjectile(closestTarget); //[cite: 19]
                nextAttackTime = Time.time + attackInterval; //[cite: 19]
            }
        }
        else //[cite: 19]
        {
            hasTargetInRange = false; //[cite: 19]
            if (movementComp != null) movementComp.SetRangedCombatHalt(false); //[cite: 19]
        }
    }

    private void FireProjectile(Transform target)
    {
        if (projectilePrefab == null) //[cite: 19]
        {
            Debug.LogWarning(gameObject.name + " is missing a Projectile Prefab reference!"); //[cite: 19]
            return; //[cite: 19]
        }

        // Determine origin point
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position; //[cite: 19]
        
        // Calculate direction towards the enemy target layout
        Vector2 shootDirection = (target.position - spawnPos).normalized; //[cite: 19]

        GameObject bulletGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity); //[cite: 19]
        Projectile projScript = bulletGO.GetComponent<Projectile>(); //[cite: 19]

        if (projScript != null) //[cite: 19]
        {
            projScript.Setup(shootDirection, projectileSpeed, attackDamage, opponentTag); //[cite: 19]
        }

        // --- NEW: PLAY AUDIO FOR RANGED ATTACK ---
        PlayAttackSound();
    }

    // --- MELEE COMBAT MECHANICS ---
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRangedUnit) return; // Ranged units ignore melee triggers[cite: 19]

        if (Time.time >= nextAttackTime) //[cite: 19]
        {
            if (collision.CompareTag(opponentTag)) //[cite: 19]
            {
                characterAttackScript unitTarget = collision.GetComponent<characterAttackScript>(); //[cite: 19]
                if (unitTarget != null) //[cite: 19]
                {
                    unitTarget.TakeDamage(Mathf.RoundToInt(attackDamage)); //[cite: 19]
                    nextAttackTime = Time.time + attackInterval; //[cite: 19]
                    
                    // --- NEW: PLAY AUDIO FOR MELEE ATTACK UNIT HIT ---
                    PlayAttackSound();
                    return; //[cite: 19]
                }

                TowerHealth towerTarget = collision.GetComponent<TowerHealth>(); //[cite: 19]
                if (towerTarget != null) //[cite: 19]
                {
                    towerTarget.TakeDamage(attackDamage); //[cite: 19]
                    nextAttackTime = Time.time + attackInterval; //[cite: 19]

                    // --- NEW: PLAY AUDIO FOR MELEE ATTACK TOWER HIT ---
                    PlayAttackSound();
                }
            }
        }
    }

    // Helper method to safely execute sound reproduction rules
    private void PlayAttackSound()
    {
        if (attackSFX != null)
        {
            AudioSource.PlayClipAtPoint(attackSFX, transform.position, sfxVolume);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount; //[cite: 19]
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); //[cite: 19]

        // Refresh text numbers immediately upon taking a hit
        UpdateHPDisplay(); //[cite: 19]

        if (currentHP <= 0) //[cite: 19]
        {
            // Give EXP award if configured
            if (givesExpWhenDead && moneyExpScript != null) //[cite: 19]
            {
                moneyExpScript.AddExp(expReward); //[cite: 19]
            }

            if (movementComp != null) movementComp.SetRangedCombatHalt(false); //[cite: 19]
            Destroy(gameObject); //[cite: 19]
        }
    }

    public void UpdateHPDisplay()
    {
        if (hpText != null) //[cite: 19]
        {
            hpText.text = currentHP.ToString();  //[cite: 19]
        }
    }

    public int GetCurrentHP()
    {
        return currentHP; //[cite: 19]
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; //[cite: 19]
        Gizmos.DrawWireSphere(transform.position, attackRange); //[cite: 19]
    }
}