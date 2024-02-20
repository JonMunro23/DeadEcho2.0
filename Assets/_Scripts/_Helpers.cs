using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _Helpers : MonoBehaviour
{
    //public static _Helpers instance;

    private void Awake()
    {
        //instance = this;
    }

    public static IEnumerator ClearTextAfterCooldown(int cooldownLength, TMP_Text textToClear)
    {
        yield return new WaitForSeconds(cooldownLength);
        textToClear.text = "";
    }

    public static IEnumerator FadeOutAudio(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
