using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "New Upgrade")]
public class Upgrade : ScriptableObject
{
    public new string name;
    [TextArea(3, 10)]
    public string description;
    public Sprite imageSprite;

    public int currentUpgradeLevel = 1;
    public int maxUpgradeLevel;

    [Header("Stat Effects (%)")]
    public float damageModifier;
    public float moveSpeedModifier;
    public float reloadSpeedModifier;
    public float fireRateModifier;

    public void LevelUp()
    {
        currentUpgradeLevel++;
    }
}
