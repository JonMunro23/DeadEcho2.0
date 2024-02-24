using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Space]
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    OptionsMenu optionsMenu;
    public static bool isPaused = false;

    private void Awake()
    {
        Unpause();
    }

    private void Update()
    {
        if(Input.GetKeyDown(pauseKey))
        {
            if (!optionsMenu.isMenuOpen)
                TogglePauseState();
        }
    }

    void TogglePauseState()
    {
        if(isPaused)
            Unpause();
        else
            Pause();
    }

    void Unpause()
    {

        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        Unpause();
    }

    public void RestartLevel()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentLevel.name);
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.OpenMenu();
    }

    public void Quit(string levelName)
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(levelName);
    }
}
