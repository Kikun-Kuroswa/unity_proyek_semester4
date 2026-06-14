using UnityEngine;

public class entityCullingScript : MonoBehaviour
{
[Tooltip("The Y coordinate at which this entity will disappear.")]
    public float cullHeight = -15f; 

    // Update is called once per frame
    void Update()
    {
        // Check if the current Y position is lower than the cullHeight
        if (transform.position.y < cullHeight)
        {
            RemoveEntity();
        }
    }

    private void RemoveEntity()
    {
        // Destroy completely removes the object from the game's memory
        Destroy(gameObject);

        /* NOTE: If you are using "Object Pooling" (a performance technique where you 
        hide and recycle objects instead of constantly destroying and creating new ones), 
        you should comment out the Destroy line above and uncomment the line below:
        */
        
        // gameObject.SetActive(false); 
    }
}
