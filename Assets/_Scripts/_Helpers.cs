using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class _Helpers : MonoBehaviour
{
    public static _Helpers instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator ClearTextAfterCooldown(int cooldownLength, TMP_Text textToClear)
    {
        yield return new WaitForSeconds(cooldownLength);
        textToClear.text = "";
    }
}
