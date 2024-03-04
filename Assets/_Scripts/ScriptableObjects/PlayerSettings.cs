using UnityEngine;

[CreateAssetMenu(fileName = "PlayerOptionsData", menuName = "New PlayerOptionsData")]
public class PlayerSettings : ScriptableObject
{
    [Header("Gameplay/Video")]
    public PlayerSettingFloat playerFov;

    [Header("Controls")]
    public PlayerSettingFloat mouseSensitivity;

    [Header("Audio")]
    public PlayerSettingFloat masterVolume;
    public PlayerSettingFloat musicVolume;
    public PlayerSettingFloat sfxVolume;
}
