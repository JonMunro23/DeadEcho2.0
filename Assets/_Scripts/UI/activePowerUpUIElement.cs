using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivePowerUpUIElement : MonoBehaviour
{
    public int powerUpDuration;
    [SerializeField]
    Transform UIParent;
    [SerializeField]
    Image spriteIcon;
    [SerializeField]
    TMP_Text text;

    Action endAction;
    Coroutine activeCoroutine;

    public void Init(PowerUpData powerUpData, Action onPowerUpEnded)
    {
        spriteIcon.sprite = powerUpData.UIIcon;
        powerUpDuration = powerUpData.powerUpDuration;
        endAction = onPowerUpEnded;
        UpdateText(powerUpDuration);
        activeCoroutine = StartCoroutine(StartPowerUpTimer(endAction));
        PlaySpawnAnimation();
    }

    void PlaySpawnAnimation()
    {
        UIParent.localScale = Vector3.zero;
        UIParent.DOScale(1, 1f);
        UIParent.DOLocalMoveY(0, 3).SetEase(Ease.OutCirc);
    }

    public void Deinit()
    {
        StopCoroutine(activeCoroutine);
        activeCoroutine = null;
        Destroy(gameObject);
    }

    public void RefreshDuration()
    {
        StopCoroutine (activeCoroutine);
        activeCoroutine = StartCoroutine(StartPowerUpTimer(endAction));
    }

    public void UpdateText(int remainingTime)
    {
        text.text = remainingTime.ToString();
    }

    public IEnumerator StartPowerUpTimer(Action onPowerUpEnded)
    {
        int counter = powerUpDuration;
        while (counter > -1)
        {
            UpdateText(counter);
            yield return new WaitForSeconds(1);
            counter--;
        }
        onPowerUpEnded?.Invoke();

        Destroy(gameObject);
    }
}
