using System;
using UnityEngine;

public class ZombieSpawnPoint : MonoBehaviour
{
    public bool isActive, isAccessible;

    public static Action<ZombieSpawnPoint, bool> onSpawnPointActivateStatusUpdated;

    private void Start()
    {
        SetIsActive(false);
    }

    public void SetIsActive(bool _isActive)
    {
        isActive = _isActive;
        onSpawnPointActivateStatusUpdated?.Invoke(this, isActive);
    }

    public void SetAccessible()
    {
        isAccessible = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("PlayerSpawnRadius"))
        {
            if (isActive)
                return;

            if(isAccessible)
                SetIsActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerSpawnRadius"))
        {
            if (isAccessible)
                SetIsActive(false);
        }
    }
}
