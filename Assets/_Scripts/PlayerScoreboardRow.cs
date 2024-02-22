using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScoreboardRow : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerNameText, playerScoreText, playerKillsText, playerHeadshotsText;

    int currentKills, currentHeadshots, currentScore;

    public void SetName(string name)
    {
        playerNameText.text = name;
    }

    public void AddKill()
    {
        currentKills++;
        playerKillsText.text = currentKills.ToString();
    }

    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        playerScoreText.text = currentScore.ToString();
    }

    public void AddHeadshot()
    {
        currentHeadshots++;
        playerHeadshotsText.text = currentHeadshots.ToString();
    }
}
