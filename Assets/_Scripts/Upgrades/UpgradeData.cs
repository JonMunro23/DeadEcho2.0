using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "New Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public enum Rarity
    {
        Common,
        Rare,
        Legendary
    };

    public Rarity upgradeRarity;

    public new string name;
    [TextArea(3, 10)]
    public string description;
    public Sprite imageSprite;
    [Tooltip("0 = Can be taken infinitely")]
    public int maxUpgradeLevel;

    [Header("Stat Effects (%)")]
    public float damageModifier;
    public float moveSpeedModifier;
    public float reloadSpeedModifier;
    public float fireRateModifier;
    public float bonusHeadshotMultiplier;
}
