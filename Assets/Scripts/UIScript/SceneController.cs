using UnityEngine;
using UnityEngine.SceneManagement; // Required to change scenes!

public class SceneController : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("The exact name of your main gameplay scene.")]
    public string gameplaySceneName = "MainScene";

    [Tooltip("The exact name of your main menu scene.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("The exact name of your game over/ending scene.")]
    public string endingSceneName = "Ending";

    /// <summary>
    /// Loads the main gameplay level. Link this to your "Play" or "Start" button.
    /// </summary>
    public void LoadGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Loads the main menu. Link this to your "Quit to Menu" or "Try Again" button.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Loads the ending scene. Call this when the player wins, loses, or beats the game.
    /// </summary>
    public void LoadEndingScene()
    {
        SceneManager.LoadScene(endingSceneName);
    }

    /// <summary>
    /// Reloads whichever scene is currently active. Great for a quick "Restart" button.
    /// </summary>
    public void RestartCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    /// <summary>
    /// Closes the game entirely. Note: This only works in a built game (.exe/.apk), not inside the Unity Editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game requested!");
        Application.Quit();
    }
}