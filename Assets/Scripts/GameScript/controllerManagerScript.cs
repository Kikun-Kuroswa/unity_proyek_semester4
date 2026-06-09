using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Don't forget to add this at the very top!

public class controllerManagerScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null)
        {
            // The new way to check for the Escape key
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("Escape key pressed!");
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
