using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class winLoseScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panel;
    public TextMeshProUGUI messageText;

    [Header("Audio Settings")]
    [Tooltip("Drag an AudioSource component here")]
    public AudioSource audioSource;
    [Tooltip("SFX clip for winning")]
    public AudioClip winSFX;
    [Tooltip("SFX clip for losing")]
    public AudioClip loseSFX;

    void Start()
    {
        panel.SetActive(false);

        // Make sure the audio source ignores the time pause!
        if (audioSource != null)
        {
            audioSource.ignoreListenerPause = true; // Crucial for paused screens
        }
    }

    public void WinLose(string result, string message)
    {
        if (result == "Win")
        {
            Debug.Log("Player has won the game!");
            messageText.text = message;
            
            // Play win sound
            PlayEndGameSFX(winSFX);

            finished();
        }
        else if (result == "Lose")
        {
            Debug.Log("Player has lost the game!");
            messageText.text = message;
            
            // Play lose sound
            PlayEndGameSFX(loseSFX);

            finished();
        }
        else
        {
            Debug.LogWarning("Unexpected result signal: " + result);
        }
    }

    private void PlayEndGameSFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // PlayOneShot lets the sound ignore the Time.timeScale = 0f pause
            audioSource.PlayOneShot(clip);
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