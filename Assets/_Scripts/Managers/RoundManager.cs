using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class RoundManager : MonoBehaviour
{
    public int currentRound;

    TMP_Text currentRoundText, roundText;

    public static Action<int> onNewRoundStarted;

    private void OnEnable()
    {
        ZombieSpawnManager.onAllZombiesKilled += StartNewRound;
    }

    private void OnDisable()
    {
        ZombieSpawnManager.onAllZombiesKilled -= StartNewRound;
    }

    private void Awake()
    {
        currentRoundText = GameObject.FindGameObjectWithTag("CurrentRoundText").GetComponent<TMP_Text>();
        roundText = currentRoundText.transform.parent.GetChild(1).GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        currentRound = 1;
        UpdateCurrentRoundText();
        PlayWaveTextIntroAnimation();
    }

    public void StartNewRound()
    {
        currentRound++;
        UpdateCurrentRoundText();
        PlayNewRoundStartedAnimation();
    }

    void UpdateCurrentRoundText()
    {
        currentRoundText.text = currentRound.ToString();
    }

    void PlayWaveTextIntroAnimation()
    {
        currentRoundText.DOColor(Color.red, 2).OnComplete(() =>
        {
            currentRoundText.transform.DOScale(.75f, 1).SetEase(Ease.OutCirc);
            roundText.DOColor(Color.clear, .3f).OnComplete(() =>
            {
                Destroy(roundText);
            }
            );

            currentRoundText.GetComponent<RectTransform>().DOLocalMove(new Vector3(50, -75, 0), 1).SetDelay(.35f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                onNewRoundStarted?.Invoke(currentRound);
            });
        });
    }

    void PlayNewRoundStartedAnimation()
    {
        currentRoundText.DOColor(Color.red, 2.5f).OnComplete(() =>
        {
            currentRoundText.DOColor(Color.white, 1f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
            {
                onNewRoundStarted?.Invoke(currentRound);
            });
        });
    }
}
