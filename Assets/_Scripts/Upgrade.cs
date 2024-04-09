[System.Serializable]
public class Upgrade
{
    public UpgradeData upgradeData;
    public int currentUpgradeLevel = 0;

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
            PlayerUpgrades.Instance.RemoveAvailableUpgrade(this);
        }
    }
}
