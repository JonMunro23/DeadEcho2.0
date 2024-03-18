using UnityEngine;
using UnityEngine.UI;

public class OptionsToggle : MonoBehaviour
{
    [SerializeField]
    PlayerSettingBool settingValue;

    [SerializeField]
    Toggle toggle;
    [SerializeField]
    Sprite toggleOnImg, toggleOffImg;

    bool previousValue, currentValue, updatedValue;


    private void OnEnable()
    {
        OptionsMenu.discardSettings += DiscardSettings;
        OptionsMenu.applySettings += ApplySettings;
        OptionsMenu.restoreDefaultSettings += RestoreDefaultSettings;
    }

    private void OnDisable()
    {
        OptionsMenu.discardSettings -= DiscardSettings;
        OptionsMenu.applySettings -= ApplySettings;
        OptionsMenu.restoreDefaultSettings -= RestoreDefaultSettings;
    }

    public void Init()
    {
        UpdateValue(settingValue.currentValue);
        currentValue = settingValue.currentValue;
        updatedValue = currentValue;
        previousValue = settingValue.currentValue;
    }

    void UpdateValue(bool newValue)
    {
        toggle.isOn = newValue;
        if (newValue)
        {
            toggle.image.sprite = toggleOnImg;
        }
        else
            toggle.image.sprite = toggleOffImg;
    }

    public void OnUpdated()
    {
        updatedValue = toggle.isOn;
        settingValue.currentValue = updatedValue;
        if (updatedValue)
        {
            toggle.image.sprite = toggleOnImg;
        }
        else
            toggle.image.sprite = toggleOffImg;
    }

    public void DiscardSettings()
    {
        updatedValue = previousValue;
        UpdateValue(updatedValue);
    }

    public void ApplySettings()
    {
        currentValue = updatedValue;
        previousValue = currentValue;
    }

    void RestoreDefaultSettings()
    {
        UpdateValue(settingValue.defaultValue);
    }

    public bool hasBeenModified()
    {
        if (updatedValue != currentValue)
        {
            return true;
        }
        else
            return false;
    }
}
