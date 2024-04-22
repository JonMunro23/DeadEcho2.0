using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct ActivePowerUp
{
    public ActivePowerUp(PowerUpType _powerUpType, ActivePowerUpUIElement _activePowerUpUIElement)
    {
        PowerUpType = _powerUpType;
        ActivePowerUpUIElement = _activePowerUpUIElement;
    }

    public PowerUpType PowerUpType;
    public ActivePowerUpUIElement ActivePowerUpUIElement;
}

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [SerializeField] List<PowerUpData> powerUpsData = new List<PowerUpData>();
    [SerializeField] List<ActivePowerUp> activePowerUps = new List<ActivePowerUp>();

    public GameObject powerUpBase;

    [SerializeField]
    GameObject powerUpUIElement;

    [SerializeField]
    Transform activePowerUpUIContainer;

    public static bool isInstantKillActive, isBottomlessClipActive, isDevastatorActive;

    Action onInstantKillEnded, onBottomlessClipEnded, onDevastatorEnded;


    private void OnEnable()
    {
        ZombieHealth.dropPowerUp += SpawnPowerUp;
        PowerUpBase.onPowerUpPickedUp += ActivatePowerUp;
        onBottomlessClipEnded += BottomlessClipEnded;
        onInstantKillEnded += InstantKillEnded;
        onDevastatorEnded += DevastatorEnded;
    }


    private void OnDisable()
    {
        ZombieHealth.dropPowerUp -= SpawnPowerUp;
        PowerUpBase.onPowerUpPickedUp -= ActivatePowerUp;
        onBottomlessClipEnded -= BottomlessClipEnded;
        onInstantKillEnded -= InstantKillEnded;
        onDevastatorEnded -= DevastatorEnded;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPowerUp(Vector3 spawnLocation)
    {
        Instantiate(GetRandomPowerUp(), spawnLocation + new Vector3(0,.9f,0), new Quaternion(0, Random.Range(0, 180), 0, 0));
    }

    GameObject GetRandomPowerUp()
    {
        int rand = Random.Range(0, powerUpsData.Count);

        GameObject randomPowerUp = powerUpBase;
        randomPowerUp.GetComponent<PowerUpBase>().powerUpData = powerUpsData[rand];
        return randomPowerUp;
    }

    public void ActivatePowerUp(PowerUpBase powerUpToActivate)
    {
        if(powerUpToActivate.powerUpData.powerUpDuration != 0)
        {
            foreach(ActivePowerUp powerUp in activePowerUps)
            {
                if(powerUpToActivate.powerUpData.PowerUpType == powerUp.PowerUpType)
                {
                    powerUp.ActivePowerUpUIElement.RefreshDuration();
                    return;
                }
            }
            ActivePowerUpUIElement activePowerUpUIElement = null;
            switch (powerUpToActivate.powerUpData.PowerUpType)
            {
                case PowerUpType.InstantKill:
                    SetInstantKillActive(true);
                    activePowerUpUIElement = SpawnActivePowerUpUIElement(powerUpToActivate.powerUpData, onInstantKillEnded);
                    break;
                case PowerUpType.BottomlessClip: 
                    SetBottomlessClipActive(true);
                    activePowerUpUIElement = SpawnActivePowerUpUIElement(powerUpToActivate.powerUpData, onBottomlessClipEnded);
                    break;
                case PowerUpType.Devastator:
                    SetDevastatorActive(true);
                    GivePowerUpWeapon(powerUpToActivate.powerUpData.powerUpWeapon);
                    activePowerUpUIElement = SpawnActivePowerUpUIElement(powerUpToActivate.powerUpData, onDevastatorEnded);
                    break;
            }

            activePowerUps.Add(new ActivePowerUp(powerUpToActivate.powerUpData.PowerUpType, activePowerUpUIElement));
            return;
        }

        switch (powerUpToActivate.powerUpData.PowerUpType)
        {
            case PowerUpType.MaxAmmo:
                GiveMaxAmmo();
                break;

        }

    }

    void GiveMaxAmmo()
    {
        WeaponSwapping.instance.RefillWeaponAmmunition();
    }

    void GivePowerUpWeapon(WeaponData weaponToGive)
    {
        WeaponSwapping.instance.GivePowerUpWeapon(weaponToGive);
    }

    ActivePowerUpUIElement SpawnActivePowerUpUIElement(PowerUpData powerUpData, Action onPowerUpEndedAction)
    {
        GameObject clone = Instantiate(powerUpUIElement, activePowerUpUIContainer);
        ActivePowerUpUIElement activePowerUpUIElement = clone.GetComponent<ActivePowerUpUIElement>();
        activePowerUpUIElement.Init(powerUpData, onPowerUpEndedAction);
        return activePowerUpUIElement;
    }

    void SetInstantKillActive(bool isActive)
    {
        isInstantKillActive = isActive;
    }

    void InstantKillEnded()
    {
        foreach (var powerUp in activePowerUps)
        {
            if(powerUp.PowerUpType == PowerUpType.InstantKill)
            {
                activePowerUps.Remove(powerUp);
                break;
            }
        }
        SetInstantKillActive(false);
    }

    void SetBottomlessClipActive(bool isActive)
    {
        isBottomlessClipActive = isActive;
    }

    void BottomlessClipEnded()
    {
        foreach (var powerUp in activePowerUps)
        {
            if (powerUp.PowerUpType == PowerUpType.BottomlessClip)
            {
                activePowerUps.Remove(powerUp);
                break;
            }
        }
        SetBottomlessClipActive(false);
    }

    void SetDevastatorActive(bool isActive)
    {
        isDevastatorActive = isActive;
    }

    void DevastatorEnded()
    {
        foreach (var powerUp in activePowerUps)
        {
            if (powerUp.PowerUpType == PowerUpType.Devastator)
            {
                activePowerUps.Remove(powerUp);
                break;
            }
        }
        WeaponSwapping.instance.RemovePowerUpWeapon();
        SetDevastatorActive(false);
    }
}
