using System;
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

    public static Action<bool> onPaused;

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
        onPaused?.Invoke(isPaused);
        pauseMenu.SetActive(false);
        if(!UpgradeSelectionMenu.isUpgradeSelectionMenuOpen)
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Pause()
    {
        isPaused = true;
        onPaused?.Invoke(isPaused);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Unpause();
    }

    public void RestartLevel()
    {
        isPaused = false;
        //onPaused?.Invoke(isPaused);
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
