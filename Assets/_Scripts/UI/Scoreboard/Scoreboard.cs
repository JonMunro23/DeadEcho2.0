using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scoreboard : MonoBehaviour
{
    [Header("Keybinds")]
    KeyCode openScoreboardKey = KeyCode.Tab;

    [Space]
    [SerializeField]
    GameObject scoreBoard;
    [SerializeField]
    PlayerScoreboardRow playerScoreboardRow;

    List<PlayerScoreboardRow> scoreboardRows = new List<PlayerScoreboardRow>();
    [SerializeField]
    TMP_Text levelNameText;

    [SerializeField]
    Transform scoreboardPlayerRowParent;

    public static bool isScoreboardOpen;

    public int numberOfPlayers = 1;

    private void OnEnable()
    {
        ZombieHealth.onDeath += UpdateKills;
        PointsManager.pointsUpdated += UpdateScore;
    }

    private void OnDisable()
    {
        ZombieHealth.onDeath -= UpdateKills;
        PointsManager.pointsUpdated -= UpdateScore;
    }

    private void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            SpawnPlayerRow(i);
        }

        UpdateLevelNameText();
    }

    void UpdateLevelNameText()
    {
        levelNameText.text = SceneManager.GetActiveScene().name;
    }

    void SpawnPlayerRow(int playerIndex)
    {
        PlayerScoreboardRow clone = Instantiate(playerScoreboardRow, scoreboardPlayerRowParent);
        scoreboardRows.Add(clone);
        clone.SetName("Player " + playerIndex);
        clone.AddScore(PointsManager.instance.currentPoints);
    }

    private void Update()
    {
        if(!PauseMenu.isPaused)
        {
            if(Input.GetKeyDown(openScoreboardKey))
            {
                ToggleScoreboard();
            }
        }
    }

    void ToggleScoreboard()
    {
        if(isScoreboardOpen)
        {
            CloseScoreboard();
        }
        else
            OpenScoreboard();
    }

    void OpenScoreboard()
    {
        isScoreboardOpen = true;
        scoreBoard.SetActive(true);
    }
    void CloseScoreboard()
    {
        isScoreboardOpen = false;
        scoreBoard.SetActive(false);
    }
    public void UpdateScore(int playerIndex, int scoreToAdd)
    {
        scoreboardRows[playerIndex].AddScore(scoreToAdd);
    }
    public void UpdateKills(int playerIndex, bool wasHeadshot)
    {
        scoreboardRows[playerIndex].AddKill();
        if(wasHeadshot)
        {
            UpdateHeadshots(playerIndex);
        }
    }

    public void UpdateHeadshots(int playerIndex)
    {
        scoreboardRows[playerIndex].AddHeadshot();
    }
}
