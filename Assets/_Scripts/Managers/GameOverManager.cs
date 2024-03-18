using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverScreen, scoreBoard, hud, player, gameOverCamera;
    [SerializeField]
    TMP_Text roundsSurvivedText;
    [SerializeField]
    Image fadeOverlay;

    [SerializeField]
    float fadeToGameOverCameraDelay, fadeToRestartLevelDelay, restartLevelDelay;
    int currentRound;

    private void OnEnable()
    {
        PlayerHealth.onDeath += EndGame;
        RoundManager.onNewRoundStarted += GetCurrentRound;
    }

    private void OnDisable()
    {
        PlayerHealth.onDeath -= EndGame;
        RoundManager.onNewRoundStarted -= GetCurrentRound;
    }

    void GetCurrentRound(int _currentRound)
    {
        currentRound = _currentRound;
    }

    void EndGame()
    {
        gameOverScreen.SetActive(true);

        roundsSurvivedText.text = "You survived " + (currentRound - 1) + " Waves!";

        FadeToGameOverCamera();

    }

    void ChangeToGameOverCamera()
    {

        FadeToRestartLevel();
    }

    void DisablePlayer()
    {
        player.SetActive(false);
    }

    void FadeToGameOverCamera()
    {
        fadeOverlay.DOColor(Color.black, 3).SetDelay(fadeToGameOverCameraDelay).OnComplete(() =>
        {
            gameOverCamera.SetActive(true);
            DisablePlayer();
            hud.SetActive(false);
            scoreBoard.SetActive(true);

            fadeOverlay.DOColor(Color.clear, 1.5f).OnComplete(() =>
            {
                ChangeToGameOverCamera();
            });
        });
    }

    void FadeToRestartLevel()
    {
        fadeOverlay.DOColor(Color.black, 3).SetDelay(fadeToRestartLevelDelay).OnComplete(() =>
        {
            StartCoroutine(RestartLevel());
        });
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(restartLevelDelay);

        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentLevel.name);
    }
}
