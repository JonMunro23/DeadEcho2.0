using System;
using UnityEngine;

public class BottomlessClip : PowerUpBase
{ 
    public static Action onBottomlessClipGrabbed;

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("Player"))
        {
            onBottomlessClipGrabbed?.Invoke();
            PowerUpManager.Instance.SetPowerUpActive(this);
        }
    }
}
