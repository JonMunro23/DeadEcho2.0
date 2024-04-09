using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "New Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public enum ModifiableWeaponStats
    {
        Damage,
        ReloadSpeed,
        FireRate,
        HeadshotMultiplier
    }
    public enum ModifiablePlayerStats
    {
        MoveSpeed,
        PointMultiplier,
        MaxHealth,
        HealthRecoveryAmount
    }

    public enum ModifiableGrenadeStats
    {
        Damage,
        BlastRadius,
        CarryAmount,
        AmountGainedPerRound
    }

    public enum WeaponTypeToAffect
    {
        All,
        Pistol,
        SMG,
        Rifle,
        Shotgun,
        Explosive,
        Melee
    }

    [System.Serializable]
    public struct WeaponStatModifier
    {
        public ModifiableWeaponStats statToModify;
        public WeaponTypeToAffect affectedWeaponType;
        public float modifyAmount;
    }

    [System.Serializable]
    public struct GrenadeStatModifier
    {
        public ModifiableGrenadeStats statToModify;
        public float modifyAmount;
    }

    [System.Serializable]
    public struct PlayerStatModifier
    {
        public ModifiablePlayerStats statToModify;
        public float modifyAmount;
    }

    public enum Rarity
    {
        Common,
        Rare,
        Legendary
    };

    [Header("General")]
    public new string name;
    [TextArea(3, 10)]
    public string description;
    public Rarity upgradeRarity;
    public Sprite imageSprite;
    [Tooltip("0 = Can be taken infinitely")]
    public int maxUpgradeLevel;

    [Header("Stat Effects (%)")]
    public PlayerStatModifier[] playerStats;
    public WeaponStatModifier[] weaponStats;
    public GrenadeStatModifier[] grenadeStats;
}
