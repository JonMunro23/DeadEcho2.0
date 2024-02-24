using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void OnEnable()
    {
        OptionsMenu.updateSettings += UpdateVolume;
    }

    private void OnDisable()
    {
        OptionsMenu.updateSettings -= UpdateVolume;
    }

    void UpdateVolume(PlayerSettings updatedSettings)
    {
        AudioListener.volume = updatedSettings.masterVolume.currentValue / 100;
    }
}
