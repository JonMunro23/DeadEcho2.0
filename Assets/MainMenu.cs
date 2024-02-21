using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame(string levelName)
    {
        SceneManager.LoadSceneAsync(levelName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenItch()
    {
        Application.OpenURL("https://biglittygames.itch.io/");
    }
}
