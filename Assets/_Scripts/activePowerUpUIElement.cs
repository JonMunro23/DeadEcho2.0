using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class activePowerUpUIElement : MonoBehaviour
{
    public int powerUpDuration;

    Action endAction;

    Coroutine activeCoroutine;

    [SerializeField]
    Transform UIParent;
    [SerializeField]
    Image spriteIcon;

    [SerializeField]
    TMP_Text text;

    public void Init(int _powerUpDuration, Sprite powerUpIconSprite ,Action actionToInvokeAtEnd)
    {
        spriteIcon.sprite = powerUpIconSprite;
        powerUpDuration = _powerUpDuration;
        endAction = actionToInvokeAtEnd;
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

    public IEnumerator StartPowerUpTimer(Action actionToInvokeAtEnd)
    {
        int counter = powerUpDuration;
        while (counter > -1)
        {
            UpdateText(counter);
            yield return new WaitForSeconds(1);
            counter--;
        }
        actionToInvokeAtEnd?.Invoke();

        Destroy(gameObject);
    }
}
