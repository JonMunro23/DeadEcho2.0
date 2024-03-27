using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [SerializeField] List<GameObject> powerUpPrefabs = new List<GameObject>();

    List<PowerUpBase> activePowerUps = new List<PowerUpBase>();

    [SerializeField]
    GameObject powerUpUIElement;

    [SerializeField]
    Transform activePowerUpContainer;

    activePowerUpUIElement activeInstantKillElement, activeBottomlessClipElement;

    [SerializeField] int powerUpDuration;

    public static Action onInstantKillEnded, onBottomlessClipEnded;
    bool instantKillStatus, bottomlessClipStatus;

    private void OnEnable()
    {
        ZombieHealth.dropPowerUp += SpawnPowerUp;
        onInstantKillEnded += InstantKillEnded;
        onBottomlessClipEnded += BottomlessClipEnded;
    }

    void InstantKillEnded()
    {
        SetInstantKillStatus(false);
    }

    void BottomlessClipEnded()
    {
        SetBottomlessClipStatus(false);
    }

    private void OnDisable()
    {
        ZombieHealth.dropPowerUp -= SpawnPowerUp;
        onInstantKillEnded -= InstantKillEnded;
        onBottomlessClipEnded -= BottomlessClipEnded;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPowerUp(Vector3 spawnLocation)
    {
        Instantiate(GetRandomPowerUp(), spawnLocation + new Vector3(0,.9f,0), new Quaternion(0, UnityEngine.Random.Range(0, 180), 0, 0));
    }

    GameObject GetRandomPowerUp()
    {
        return powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Count)];
    }

    public void SetPowerUpActive(PowerUpBase powerUp)
    {
        switch (powerUp.powerUpType)
        {
            case PowerUpType.InstantKill:
                if (GetInstantKillStatus())
                {
                    activeInstantKillElement.RefreshDuration();
                    break;
                }
                SpawnPowerUpUIElement(powerUp);
                activePowerUps.Add(powerUp);
                SetInstantKillStatus(true);
                break;
            case PowerUpType.BottomlessClip:
                if(GetBottomlessClipStatus())
                {
                    activeBottomlessClipElement.RefreshDuration();
                    break;
                }
                SpawnPowerUpUIElement(powerUp);
                activePowerUps.Add(powerUp);
                SetBottomlessClipStatus(true);
                break;
        }
    }



    void SpawnPowerUpUIElement(PowerUpBase _activePowerUp)
    {
        GameObject clone = Instantiate(powerUpUIElement, activePowerUpContainer);
        var activePowerUpUIElement = clone.GetComponent<activePowerUpUIElement>();
        switch (_activePowerUp.powerUpType)
        {
            case PowerUpType.InstantKill:
                activeInstantKillElement = activePowerUpUIElement;
                break;
            case PowerUpType.BottomlessClip:
                activeBottomlessClipElement = activePowerUpUIElement;
                break;
        }
        activePowerUpUIElement.Init(powerUpDuration, _activePowerUp.powerUpUIIcon, GetActionToInvokeOnEnd(_activePowerUp));
    }

    Action GetActionToInvokeOnEnd(PowerUpBase _activePowerUp)
    {
        switch (_activePowerUp.powerUpType)
        {
            case PowerUpType.InstantKill:
                return onInstantKillEnded;
            case PowerUpType.BottomlessClip:
                return onBottomlessClipEnded;
            default:
                return null;
        }
    }

    void SetInstantKillStatus(bool status)
    {
        instantKillStatus = status;
    }

    void SetBottomlessClipStatus(bool status)
    {
        bottomlessClipStatus = status;
    }

    public bool GetInstantKillStatus()
    {
        return instantKillStatus;
    }

    public bool GetBottomlessClipStatus()
    {
        return bottomlessClipStatus;
    }
}
