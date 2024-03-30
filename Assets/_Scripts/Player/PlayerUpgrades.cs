using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public UpgradeData upgradeData;
    public int currentUpgradeLevel;

    public Upgrade(UpgradeData upgradeData, int upgradeLevel)
    {
        this.upgradeData = upgradeData;
        this.currentUpgradeLevel = upgradeLevel;
    }

    public int GetCurrentLevel()
    {
        return currentUpgradeLevel;
    }

    public void LevelUp()
    {
        currentUpgradeLevel++;
        CheckIsMaxRank();
    }

    public void CheckIsMaxRank()
    {
        if (upgradeData.maxUpgradeLevel != 0 && GetCurrentLevel() == upgradeData.maxUpgradeLevel)
        {
            PlayerUpgrades.Instance.RemoveAvailableUpgrade(upgradeData);
        }
    }
}


public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    public List<UpgradeData> allUpgrades = new List<UpgradeData>();
    public List<UpgradeData> availableUpgrades = new List<UpgradeData>();

    public static float damageModifier, moveSpeedModifier, reloadSpeedModifier, fireRateModifier, bonusheadshotMultiplier;

    public static Action onUpgradesRefreshed;

    public List<Upgrade> collectedUpgrades = new List<Upgrade>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        availableUpgrades.AddRange(allUpgrades);
    }

    public void RefreshModifiers()
    {
        damageModifier = 0;
        moveSpeedModifier = 0;
        reloadSpeedModifier = 0;
        fireRateModifier = 0;
        bonusheadshotMultiplier = 0;

        foreach (Upgrade upgrade in collectedUpgrades)
        {
            damageModifier += upgrade.upgradeData.damageModifier * upgrade.currentUpgradeLevel;
            moveSpeedModifier += upgrade.upgradeData.moveSpeedModifier * upgrade.currentUpgradeLevel;
            reloadSpeedModifier += upgrade.upgradeData.reloadSpeedModifier * upgrade.currentUpgradeLevel;
            fireRateModifier += upgrade.upgradeData.fireRateModifier * upgrade.currentUpgradeLevel;
            bonusheadshotMultiplier += upgrade.upgradeData.bonusHeadshotMultiplier * upgrade.currentUpgradeLevel;
        }

        onUpgradesRefreshed?.Invoke();
    }

    public void AddUpgradeToCollection(UpgradeData upgradeToAdd)
    {
        foreach (Upgrade collectedUpgrade in collectedUpgrades)
        {
            if(collectedUpgrade.upgradeData == upgradeToAdd)
            {
                collectedUpgrade.LevelUp();
                RefreshModifiers();
                return;
            }
        }
        Upgrade newUpgrade = new Upgrade(upgradeToAdd, 1);
        collectedUpgrades.Add(newUpgrade);
        newUpgrade.CheckIsMaxRank();
        RefreshModifiers();
    }

    public void RemoveAvailableUpgrade(UpgradeData upgradeToRemove)
    {
        availableUpgrades.Remove(upgradeToRemove);
        UpgradeSelectionMenu.instance.RefreshAvailableUpgrades();
    }

    public int GetCurrentUpgradeRank(UpgradeData upgrade)
    {
        foreach(Upgrade collectedUpgrade in collectedUpgrades)
        {
            if(upgrade == collectedUpgrade.upgradeData)
            {
                return collectedUpgrade.GetCurrentLevel();
            }
        }

        return 0;
    }

    //public static void ApplyUpgrade(UpgradeData upgradeToApply)
    //{
    //    damageModifier += upgradeToApply.damageModifier;
    //    moveSpeedModifier += upgradeToApply.moveSpeedModifier;
    //    reloadSpeedModifier += upgradeToApply.reloadSpeedModifier;
    //    fireRateModifier += upgradeToApply.fireRateModifier;
    //    headshotDamageModifier += upgradeToApply.headshotDamageModifier;

    //    onUpgradesRefreshed?.Invoke();
    //}

}
