using UnityEngine;
using TMPro; // If you are using TextMeshPro for the end screen text

public class EndingSceneDisplay : MonoBehaviour
{
    [Header("UI Text Reference")]
    public TextMeshProUGUI resultText; // Drag your victory/defeat text here

    void Start()
    {
        // Read the static variable from the SceneController
        string result = SceneController.gameResultSignal;

        if (result == "Win")
        {
            if (resultText != null) resultText.text = "VICTORY!";
            Debug.Log("The Ending Scene received: PLAYER WON");
            // Turn on your Victory UI Panels here...
        }
        else if (result == "Lose")
        {
            if (resultText != null) resultText.text = "GAME OVER";
            Debug.Log("The Ending Scene received: PLAYER LOST");
            // Turn on your Defeat UI Panels here...
        }
        else
        {
            if (resultText != null) resultText.text = "End of Game";
            Debug.LogWarning("No match signal was received.");
        }
    }
}