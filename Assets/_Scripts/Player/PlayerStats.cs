using System;

public class PlayerStats
{
    public static float damageModifier, moveSpeedModifier, reloadSpeedModifier, fireRateModifier;

    public static Action onUpgradeApplied;

    public static void ApplyUpgrade(Upgrade upgradeToApply)
    {
        damageModifier += upgradeToApply.damageModifier * upgradeToApply.currentUpgradeLevel;
        moveSpeedModifier += upgradeToApply.moveSpeedModifier * upgradeToApply.currentUpgradeLevel;
        reloadSpeedModifier += upgradeToApply.reloadSpeedModifier * upgradeToApply.currentUpgradeLevel;
        fireRateModifier += upgradeToApply.fireRateModifier * upgradeToApply.currentUpgradeLevel;

        onUpgradeApplied?.Invoke();
    }

}
