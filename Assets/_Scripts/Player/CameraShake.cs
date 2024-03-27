using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = .5f;
    public float strength = .1f;

    private void OnEnable()
    {
        PlayerHealth.onDamageTaken += Shake;
    }

    private void OnDisable()
    {
        PlayerHealth.onDamageTaken -= Shake;
    }

    public void Shake()
    {
        transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }
}
