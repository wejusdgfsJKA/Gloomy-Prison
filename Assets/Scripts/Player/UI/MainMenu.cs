using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    protected void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// Fires when the Start button is pressed.
    /// </summary>
    public void OnStartButton()
    {
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// Fires when the Exit button is pressed.
    /// </summary>
    public void OnExitButton()
    {
        Application.Quit();
    }
}
