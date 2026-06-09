using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private string targetTag;
    private Vector2 direction;

    /// <summary>
    /// Initializes the projectile's attributes from the attacker's script.
    /// </summary>
    public void Setup(Vector2 launchDirection, float projectileSpeed, float projectileDamage, string opponentTag)
    {
        this.direction = launchDirection.normalized;
        this.speed = projectileSpeed;
        this.damage = projectileDamage;
        this.targetTag = opponentTag;

        // Automatically destroy the projectile after 5 seconds if it misses everything to prevent clutter
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Move the projectile forward over time
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if it hit an enemy unit
        if (collision.CompareTag(targetTag))
        {
            characterAttackScript unit = collision.GetComponent<characterAttackScript>();
            if (unit != null)
            {
                unit.TakeDamage(Mathf.RoundToInt(damage));
                Destroy(gameObject); // Destroy the bullet
                return;
            }

            // 2. Check if it hit an opponent's Tower
            TowerHealth tower = collision.GetComponent<TowerHealth>();
            if (tower != null)
            {
                tower.TakeDamage(damage);
                Destroy(gameObject); // Destroy the bullet
                return;
            }
        }
    }
}