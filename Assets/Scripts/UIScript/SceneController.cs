using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Static variables persist across scene changes
    public static string gameResultSignal = ""; // Can be "Win" or "Lose"
    public winLoseScript winLosePanel; // Reference to the win/lose panel script

    /// <summary>
    /// Call this to change to the Ending scene and pass whether the player won or lost.
    /// </summary>
    public void LoadEndingScene(bool playerWon)
    {
        if (playerWon)
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

        // // Make sure "Ending" matches the exact name of your scene asset in Build Settings!
        // SceneManager.LoadScene("Ending"); 
    }

    // Your existing scene navigation functions can remain below...
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}