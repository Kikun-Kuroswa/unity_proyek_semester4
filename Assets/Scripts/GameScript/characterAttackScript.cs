using UnityEngine;
using TMPro; // REQUIRED to control TextMeshPro text elements!

public class characterAttackScript : MonoBehaviour
{
    [Header("Base Combat Stats")]
    public int maxHP = 100;
    [SerializeField] private int currentHP;
    public float attackDamage = 10f;
    public float attackInterval = 1.5f;
    protected float nextAttackTime;

    [Header("UI Text Reference")]
    [Tooltip("Drag your floating Canvas TextMeshPro component here.")]
    public TextMeshProUGUI hpText; 

    [Header("Ranged Specialist Settings")]
    public bool isRangedUnit = false;
    [Tooltip("The visual bullet prefab that will fly towards targets.")]
    public GameObject projectilePrefab;
    [Tooltip("The point relative to the character where the bullet spawns (e.g. hands or weapon tip).")]
    public Transform firePoint;
    public float attackRange = 5f;
    public float projectileSpeed = 8f;

    [Header("Target Layer Filtering")]
    [Tooltip("Include the layers that units and towers occupy (e.g., Default, UI, or custom layers).")]
    public LayerMask targetLayers;

    [Header("Exp Settings")]
    public int expReward = 25;
    public bool givesExpWhenDead = true; 

    [Header("Outside Script Connections")]
    public moneyExpScript moneyExpScript; 

    private string opponentTag;
    private movementScript movementComp;
    private bool hasTargetInRange = false;

    void Start()
    {
		if (UpgradeManager.Instance != null)
		{
			// 1. Calculate and add upgraded HP to default base stats
			maxHP += UpgradeManager.Instance.hpUpgradeLevel * UpgradeManager.Instance.hpIncrementPerLevel;
			
			// 2. Calculate and add upgraded Damage
			attackDamage += UpgradeManager.Instance.damageUpgradeLevel * UpgradeManager.Instance.damageIncrementPerLevel;
		}
		
        // Set current HP to max HP when spawning
        currentHP = maxHP;
        
        // Force text component to change from template strings to actual numbers!
        UpdateHPDisplay();

        movementComp = GetComponent<movementScript>();
        moneyExpScript = Object.FindAnyObjectByType<moneyExpScript>();

        // Determine who our enemies are based on our own Tag assignment
        opponentTag = gameObject.CompareTag("soldier") ? "Enemy" : "soldier";
        
        if (moneyExpScript == null)
        {
            Debug.LogWarning(gameObject.name + " spawned, but couldn't find a moneyExpScript in the scene!");
        }
    }

    void Update()
    {
        if (isRangedUnit)
        {
            HandleRangedCombat();
        }
    }

    private void HandleRangedCombat()
    {
        // Scan the surrounding space in a radius matching the attack range
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayers);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in hitTargets)
        {
            // Verify if the scanned object belongs to the enemy side
            if (col.CompareTag(opponentTag))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = col.transform;
                }
            }
        }

        if (closestTarget != null)
        {
            hasTargetInRange = true;

            // Signal the movement script to halt walking behavior
            if (movementComp != null) movementComp.SetRangedCombatHalt(true);

            // Execute attack loop when ready
            if (Time.time >= nextAttackTime)
            {
                FireProjectile(closestTarget);
                nextAttackTime = Time.time + attackInterval;
            }
        }
        else
        {
            hasTargetInRange = false;
            if (movementComp != null) movementComp.SetRangedCombatHalt(false);
        }
    }

    private void FireProjectile(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(gameObject.name + " is missing a Projectile Prefab reference!");
            return;
        }

        // Determine origin point
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        
        // Calculate direction towards the enemy target layout
        Vector2 shootDirection = (target.position - spawnPos).normalized;

        GameObject bulletGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Projectile projScript = bulletGO.GetComponent<Projectile>();

        if (projScript != null)
        {
            projScript.Setup(shootDirection, projectileSpeed, attackDamage, opponentTag);
        }
    }

    // --- MELEE COMBAT MECHANICS ---
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRangedUnit) return; // Ranged units ignore melee triggers

        if (Time.time >= nextAttackTime)
        {
            if (collision.CompareTag(opponentTag))
            {
                characterAttackScript unitTarget = collision.GetComponent<characterAttackScript>();
                if (unitTarget != null)
                {
                    unitTarget.TakeDamage(Mathf.RoundToInt(attackDamage));
                    nextAttackTime = Time.time + attackInterval;
                    return;
                }

                TowerHealth towerTarget = collision.GetComponent<TowerHealth>();
                if (towerTarget != null)
                {
                    towerTarget.TakeDamage(attackDamage);
                    nextAttackTime = Time.time + attackInterval;
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // Refresh text numbers immediately upon taking a hit
        UpdateHPDisplay();

        if (currentHP <= 0)
        {
            // Give EXP award if configured
            if (givesExpWhenDead && moneyExpScript != null)
            {
                moneyExpScript.AddExp(expReward);
            }

            if (movementComp != null) movementComp.SetRangedCombatHalt(false);
            Destroy(gameObject);
        }
    }

    public void UpdateHPDisplay()
    {
        if (hpText != null)
        {
            hpText.text = currentHP.ToString(); 
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    // Draws the attack circle inside Unity Editor scene view window for easy configuration layout checking
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}