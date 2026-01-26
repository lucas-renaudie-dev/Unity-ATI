using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject optionsScreen;

    void Start()
    {   
        mainMenuScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(true);
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;

        GameDifficultySettings.Instance.ApplyDifficulty();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Arena Game");
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void Options()
    {
        mainMenuScreen.gameObject.SetActive(false);
        optionsScreen.gameObject.SetActive(true);
    }

    public void Back()
    {
        optionsScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(true);
        
    }
}
