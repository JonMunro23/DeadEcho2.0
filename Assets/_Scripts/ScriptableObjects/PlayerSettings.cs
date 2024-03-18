using UnityEngine;

[CreateAssetMenu(fileName = "PlayerOptionsData", menuName = "PlayerSettings/New PlayerOptionsData")]
public class PlayerSettings : ScriptableObject
{
    [Header("Gameplay/Video")]
    public PlayerSettingFloat playerFov;

    [Header("Video")]
    public PlayerSettingBool fullscreen;
    public FullScreenMode fullScreenMode;

    [Header("Controls")]
    public PlayerSettingFloat mouseSensitivity;

    [Header("Audio")]
    public PlayerSettingFloat masterVolume;
    public PlayerSettingFloat musicVolume;
    public PlayerSettingFloat sfxVolume;
}
