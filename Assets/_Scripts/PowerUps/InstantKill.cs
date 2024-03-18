using DG.Tweening;
using System;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;

public class InstantKill : PowerUpBase
{ 
    public static Action onInstantKillGrabbed;


    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("Player"))
        {
            onInstantKillGrabbed?.Invoke();
            PowerUpManager.Instance.SetPowerUpActive(this);
        }
    }
}
