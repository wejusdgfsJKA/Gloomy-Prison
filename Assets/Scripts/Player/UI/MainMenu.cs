using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    protected void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnStartButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
}
