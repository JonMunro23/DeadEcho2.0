using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Keybinds")]
    KeyCode closeMenuKey = KeyCode.Escape;

    public static Action<PlayerSettings> updateSettings;
    public static Action discardSettings;
    public static Action applySettings;
    public static Action restoreDefaultSettings;

    GameObject menuContainer;

    [SerializeField]
    GameObject modifiedSettingsPopup;

    public bool isMenuOpen = false;
    bool isPopupOpen;

    [SerializeField]
    PlayerSettings playerOptionsData;

    [SerializeField]
    List<OptionsSlider> optionsSliders = new List<OptionsSlider>();
    [SerializeField]
    List<OptionsToggle> optionsToggles = new List<OptionsToggle>();

    private void Awake()
    {
        menuContainer = transform.GetChild(0).gameObject;
    }

    void Start()
    {
        CloseMenu();
        ApplySettings();
    }

    private void Update()
    {
        if(Input.GetKeyDown(closeMenuKey))
        {
            if(isPopupOpen)
            {
                CloseModifiedSettingsPopup();
                return;
            }

            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        isMenuOpen = true;
        menuContainer.SetActive(true);
        InitOptionsMenu();
    }

    public void CloseMenu()
    {
        if(HasModifiedSettings())
        {
            ShowModifiedSettingsPopup();
        }
        else
        {
            isMenuOpen = false;
            menuContainer.SetActive(false);
        }

    }

    void ShowModifiedSettingsPopup()
    {
        isPopupOpen = true;
        modifiedSettingsPopup.SetActive(true);
    }

    void CloseModifiedSettingsPopup()
    {
        isPopupOpen = false;
        modifiedSettingsPopup.SetActive(false);
    }

    void InitOptionsMenu()
    {
        InitSliders();
        InitToggles();
    }

    void InitSliders()
    {
        foreach (OptionsSlider slider in optionsSliders)
        {
            slider.Init();
        }
    }

    void InitToggles()
    {
        foreach (OptionsToggle toggle in optionsToggles)
        {
            toggle.Init();
        }
    }

    public void ApplySettings()
    {
        applySettings?.Invoke();
        updateSettings?.Invoke(playerOptionsData);
    }

    public void DiscardSettings()
    {
        discardSettings?.Invoke();
    }

    public void ApplySettingsViaPopup()
    {
        CloseModifiedSettingsPopup();
        applySettings?.Invoke();
        updateSettings?.Invoke(playerOptionsData);
        CloseMenu();
    }

    public void DiscardSettingsViaPopup()
    {
        CloseModifiedSettingsPopup();
        discardSettings?.Invoke();
        CloseMenu();
    }

    public void RestoreDefaultSettings()
    {
        restoreDefaultSettings?.Invoke();
    }

    bool HasModifiedSettings()
    {
        foreach (OptionsSlider slider in optionsSliders)
        {
            if(slider.hasBeenModified() == true)
            {
                return true;
            }
        }

        return false;
    }
}
