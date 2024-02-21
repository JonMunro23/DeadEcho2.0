using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [Header("Keybinds")]
    KeyCode openScoreboardKey = KeyCode.Tab;

    [Space]
    [SerializeField]
    GameObject scoreBoard;
    public static bool isScoreboardOpen;

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
    public void AddNewPlayer()
    {

    }
    public void UpdateScore()
    {

    }
    public void UpdateKills()
    {

    }
    public void UpdateHeadshots()
    {

    }
}
