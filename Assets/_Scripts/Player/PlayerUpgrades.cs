using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    public List<UpgradeData> allUpgradeData = new List<UpgradeData>();
    public List<Upgrade> availableUpgrades = new List<Upgrade>();

    //GrenadeStats
    public static float grenadeDamageModifier, blastRadiusModifier;
    public static int grenadeCarryAmountModifier, grenadesGainedPerRoundModifier;

    //WeaponStats
    public static float weaponDamageModifier, reloadSpeedModifier, fireRateModifier, bonusheadshotMultiplier;

    //PlayerStats
    public static float moveSpeedModifier, maxHealthModifier, healthRecoverModifier, pointMultiplier;

    public static Action onUpgradesRefreshed;

    public List<Upgrade> collectedUpgrades = new List<Upgrade>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (UpgradeData upgrade in allUpgradeData)
        {
            availableUpgrades.Add(new Upgrade(upgrade, 0));
        }
    }

    public void RefreshModifiers()
    {
        //grenadeModifiers
        grenadeDamageModifier = 0;
        blastRadiusModifier = 0;
        grenadeCarryAmountModifier = 0;
        grenadesGainedPerRoundModifier = 0;

        //weaponModifiers
        weaponDamageModifier = 0;
        reloadSpeedModifier = 0;
        fireRateModifier = 0;
        bonusheadshotMultiplier = 0;

        //PlayerModifiers
        moveSpeedModifier = 0;
        maxHealthModifier = 0;
        healthRecoverModifier = 0;
        pointMultiplier = 0;

        foreach (Upgrade upgrade in collectedUpgrades)
        {
            if(upgrade.upgradeData.weaponStats.Length > 0)
            {
                foreach (UpgradeData.WeaponStatModifier weaponStatModifier in upgrade.upgradeData.weaponStats)
                {
                    UpdateWeaponStats(upgrade, weaponStatModifier);
                }
            }

            if(upgrade.upgradeData.playerStats.Length > 0)
            {
                foreach (UpgradeData.PlayerStatModifier playerStatModifier in upgrade.upgradeData.playerStats)
                {
                    UpdatePlayerStats(upgrade, playerStatModifier);
                }
            }

            if (upgrade.upgradeData.grenadeStats.Length > 0)
            {
                foreach (UpgradeData.GrenadeStatModifier grenadeStatModifier in upgrade.upgradeData.grenadeStats)
                {
                    UpdateGrenadeStats(upgrade, grenadeStatModifier);
                }
            }
        }

        onUpgradesRefreshed?.Invoke();
    }

    void UpdateWeaponStats(Upgrade upgrade, UpgradeData.WeaponStatModifier weaponStatModifier)
    {
        switch (weaponStatModifier.statToModify)
        {
            case UpgradeData.ModifiableWeaponStats.Damage:
                weaponDamageModifier += weaponStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiableWeaponStats.ReloadSpeed:
                reloadSpeedModifier += weaponStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiableWeaponStats.FireRate:
                fireRateModifier += weaponStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiableWeaponStats.HeadshotMultiplier:
                bonusheadshotMultiplier += weaponStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
        }
    }

    void UpdateGrenadeStats(Upgrade upgrade, UpgradeData.GrenadeStatModifier grenadeStatModifier)
    {
        switch (grenadeStatModifier.statToModify)
        {
            case UpgradeData.ModifiableGrenadeStats.Damage:
                grenadeDamageModifier += grenadeStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiableGrenadeStats.BlastRadius:
                blastRadiusModifier += grenadeStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiableGrenadeStats.CarryAmount:
                grenadeCarryAmountModifier += Mathf.RoundToInt(grenadeStatModifier.modifyAmount * upgrade.currentUpgradeLevel);
                break;
            case UpgradeData.ModifiableGrenadeStats.AmountGainedPerRound:
                grenadesGainedPerRoundModifier += Mathf.RoundToInt(grenadeStatModifier.modifyAmount * upgrade.currentUpgradeLevel);
                break;
        }
    }

    void UpdatePlayerStats(Upgrade upgrade, UpgradeData.PlayerStatModifier playerStatModifier)
    {
        switch (playerStatModifier.statToModify)
        {
            case UpgradeData.ModifiablePlayerStats.MaxHealth:
                maxHealthModifier += playerStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiablePlayerStats.HealthRecoveryAmount:
                healthRecoverModifier += playerStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiablePlayerStats.PointMultiplier:
                pointMultiplier += playerStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
            case UpgradeData.ModifiablePlayerStats.MoveSpeed:
                moveSpeedModifier += playerStatModifier.modifyAmount * upgrade.currentUpgradeLevel;
                break;
        }
    }

    public void AddUpgradeToCollection(Upgrade upgradeToAdd)
    {
        foreach (Upgrade collectedUpgrade in collectedUpgrades)
        {
            if(collectedUpgrade == upgradeToAdd)
            {
                collectedUpgrade.LevelUp();
                RefreshModifiers();
                return;
            }
        }
        upgradeToAdd.LevelUp();
        collectedUpgrades.Add(upgradeToAdd);
        upgradeToAdd.CheckIsMaxRank();
        RefreshModifiers();
    }

    public void RemoveAvailableUpgrade(Upgrade upgradeToRemove)
    {
        availableUpgrades.Remove(upgradeToRemove);
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

    public List<Upgrade> GetCollectedUpgrades()
    {
        return collectedUpgrades;
    }
}
