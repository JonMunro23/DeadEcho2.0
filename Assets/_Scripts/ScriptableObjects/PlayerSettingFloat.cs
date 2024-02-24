using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingFloat", menuName = "New PlayerSettingFloat")]
public class PlayerSettingFloat : PlayerSetting
{
    public float defaultValue;
    public float currentValue;
    public float valueMin;
    public float valueMax;
}
