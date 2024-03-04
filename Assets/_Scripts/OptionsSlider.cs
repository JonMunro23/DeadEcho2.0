using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsSlider : MonoBehaviour
{
    [SerializeField]
    PlayerSettingFloat settingValue;

    [Space]
    [SerializeField]
    TMP_InputField sliderInputField;
    [SerializeField]
    TMP_Text sliderLabel;
    [SerializeField]
    Slider slider;

    //public UnityEvent<float> onUpdated;

    float previousValue, currentValue, updatedValue;

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

    public void InitSlider()
    {
        sliderLabel.text = settingValue.name;
        slider.minValue = settingValue.valueMin;
        slider.maxValue = settingValue.valueMax;
        UpdateSliderValue(settingValue.currentValue);
        currentValue = settingValue.currentValue;
        updatedValue = currentValue;
        previousValue = settingValue.currentValue;
    }

    void UpdateSliderValue(float newValue)
    {
        slider.value = newValue;
        sliderInputField.text = newValue.ToString();
    }

    public void OnSliderUpdated()
    {
        updatedValue = slider.value;
        sliderInputField.text = updatedValue.ToString();
        settingValue.currentValue = updatedValue;
    }

    public void OnInputFieldTextSubmit()
    {
        updatedValue = int.Parse(sliderInputField.text);
        slider.value = updatedValue;
        settingValue.currentValue = updatedValue;
    }

    public void DiscardSettings()
    {
        updatedValue = previousValue;
        UpdateSliderValue(updatedValue);
    }

    public void ApplySettings()
    {
        currentValue = updatedValue;
        previousValue = currentValue;
    }

    void RestoreDefaultSettings()
    {
        UpdateSliderValue(settingValue.defaultValue);
    }

    public bool hasBeenModified()
    {
        if (updatedValue != currentValue)
        {
            Debug.Log(settingValue.name + ": " +  updatedValue + " is not equal to current value: " + currentValue);
            return true;
        }
        else
            return false;
    }
}
