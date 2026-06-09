using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class winLoseScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panel;
    public TextMeshProUGUI messageText;

    void Start()
    {
        panel.SetActive(false);
    }

    public void WinLose(string result, string message)
    {
        if (result == "Win")
        {
            Debug.Log("Player has won the game!");
            messageText.text = message;
            finished();
        }
        else if (result == "Lose")
        {
            Debug.Log("Player has lost the game!");
            messageText.text = message; 
            finished();
        }
        else
        {
            Debug.LogWarning("Unexpected result signal: " + result);
        }

    }

    public void finished()
    {
        panel.SetActive(true);
        Time.timeScale = 0f; // pause game
    }

    public void restartGame()
    {
        Time.timeScale = 1f; // resume game
        panel.SetActive(false);
        SceneManager.LoadScene("MainScene");
    }

    public void mainMenu()
    {
        Time.timeScale = 1f; // resume game
        panel.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
