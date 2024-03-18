using UnityEngine;

public class DisplaySettingsManager : MonoBehaviour
{
    private void OnEnable()
    {
        OptionsMenu.updateSettings += UpdateFullscreen;
    }

    private void OnDisable()
    {
        OptionsMenu.updateSettings -= UpdateFullscreen;
    }

    void UpdateFullscreen(PlayerSettings playerSettings)
    {
        Screen.fullScreen = playerSettings.fullscreen.currentValue;
    }
}
