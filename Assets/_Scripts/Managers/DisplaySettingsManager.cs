using UnityEngine;

public class DisplaySettingsManager : MonoBehaviour
{
    //public int target = 30;

    //void Awake()
    //{
    //    QualitySettings.vSyncCount = 0;
    //    Application.targetFrameRate = target;
    //}

    //void Update()
    //{
    //    if (Application.targetFrameRate != target)
    //        Application.targetFrameRate = target;
    //}

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
