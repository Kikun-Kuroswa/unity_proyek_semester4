using UnityEngine;

public class ignoreAndDieScript : MonoBehaviour
{
    [Header("Timer Settings")]
    // How many seconds this object will live before disappearing
    public float lifetime = 3f; 

    [Header("Phase Settings")]
    // You can type as many tags as you want into this list in the Inspector!
    public string[] tagsToIgnore = { "Soldier", "Enemy" };

    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();

        // 1. THE DEATH TIMER
        // Destroy normally kills an object instantly, but if you add a comma and a number,
        // it acts as a built-in countdown timer!
        Destroy(gameObject, lifetime);

        // 2. THE PHASING LOGIC
        IgnoreSpecificTags();
    }

    private void IgnoreSpecificTags()
    {
        // Safety check: Make sure this object actually has a collider
        if (myCollider == null) return;

        // Loop through every single tag you typed into the Inspector list
        foreach (string tagToCheck in tagsToIgnore)
        {
            // Find every object currently in the game that has this sticky note
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagToCheck);

            // Loop through all those objects we just found
            foreach (GameObject obj in objectsWithTag)
            {
                Collider2D otherCollider = obj.GetComponent<Collider2D>();

                // If they have a collider, tell the physics engine to ignore them
                if (otherCollider != null)
                {
                    Physics2D.IgnoreCollision(this.myCollider, otherCollider);
                }
            }
        }
    }
}