using System;

public class PlayerStats
{
    public static float damageModifier, moveSpeedModifier, reloadSpeedModifier, fireRateModifier;

    public static Action onUpgradeApplied;

    public static void ApplyUpgrade(Upgrade upgradeToApply)
    {
        damageModifier += upgradeToApply.damageModifier;
        moveSpeedModifier += upgradeToApply.moveSpeedModifier;
        reloadSpeedModifier += upgradeToApply.reloadSpeedModifier;
        fireRateModifier += upgradeToApply.fireRateModifier;

        onUpgradeApplied?.Invoke();
    }

}
