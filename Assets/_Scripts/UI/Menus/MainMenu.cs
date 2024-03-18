using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject loadingScreen;
    [SerializeField]
    Slider loadingBar;
    [SerializeField]
    TMP_Text loadingText;

    AsyncOperation loadingOperation;

    bool isLoadingFinished;

    private void Update()
    {
        if(isLoadingFinished)
        {
            if(Input.anyKeyDown)
                loadingOperation.allowSceneActivation = true;
        }
    }

    public void StartGame(string levelName)
    {
        StartCoroutine(LoadLevelAsync(levelName));
    }

    IEnumerator LoadLevelAsync(string sceneName)
    {
        loadingScreen.SetActive(true);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                loadingText.text = "Press Any Key To Continue";
                isLoadingFinished = true;
            }

            float progress = Mathf.Clamp01(loadingOperation.progress / .9f);
            loadingBar.value = progress;

            yield return null;
        }


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
