using System;
using UnityEngine;

public class MaxAmmo : PowerUpBase
{ 
    public static Action onMaxAmmoGrabbed;

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("Player"))
        {
            onMaxAmmoGrabbed?.Invoke();
        }
    }
}
