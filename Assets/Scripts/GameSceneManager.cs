using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("UI Screens")]
    public GameObject pauseScreen;
    public GameObject deathScreen;
    public GameObject pauseOptionsScreen;
    public GameObject deathOptionsScreen;

    private bool isPaused = false;
    public Image damageOverlay;
    public Text comboText;
    public bool inputEnabled = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1f;
        inputEnabled = true;

        pauseScreen.SetActive(false);
        deathScreen.SetActive(false);
        pauseOptionsScreen.SetActive(false);
        deathOptionsScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =========================
    // RESUME (Pause Menu)
    // =========================
    public void ResumeGame()
    {
        isPaused = false;
        inputEnabled = true;

        pauseScreen.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =========================
    // PLAY AGAIN (Death Screen)
    // =========================
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        inputEnabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Arena Game");
    }

    // =========================
    // QUIT GAME
    // =========================
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }


    // =========================
    // PAUSE TOGGLE (ESC)
    // =========================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathScreen.activeSelf)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        inputEnabled = !isPaused;

        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }


    public void PlayerDied()
    {
        comboText.gameObject.SetActive(false);
        inputEnabled = false;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        damageOverlay.gameObject.SetActive(false);
        deathScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("Main Menu");
    }

    public void PauseOptions()
    {
        pauseScreen.gameObject.SetActive(false);
        pauseOptionsScreen.gameObject.SetActive(true);
    }
    public void BackToPauseScreen()
    {
        pauseOptionsScreen.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(true);
    }

    public void DeathOptions()
    {
        deathScreen.gameObject.SetActive(false);
        deathOptionsScreen.gameObject.SetActive(true);
    }

    public void BackToDeathScreen()
    {
        deathOptionsScreen.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(true);
    }
}
