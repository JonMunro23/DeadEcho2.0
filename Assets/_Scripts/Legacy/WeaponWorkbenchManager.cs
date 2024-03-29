using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWorkbenchManager : MonoBehaviour
{
    public static WeaponWorkbenchManager instance;

    [SerializeField] WorkbenchUIWeaponHolder primarySlot1WeaponHolder, primarySlot2WeaponHolder, SecondarySlotWeaponHolder;
    [SerializeField] TMP_Text pointsText;
    [SerializeField] Transform weaponModelSpawnPos;

    GameObject workbenchUI;
    GameObject currentlyDisplayedWeaponObj;

    [Header("WeaponStats")]
    [SerializeField] GameObject weaponStatsPanel;
    [SerializeField] TMP_Text wepNameText, wepDmgText, wepAccText, wepFireRateText, wepReloadSpeedText, wepMagSizeText, wepReserveAmmoText;

    [Header("Attachments")]
    [SerializeField] GameObject AttachmentsPanel;
    [SerializeField] GameObject sightAttachmentHolder;
    [SerializeField] GameObject magAttachmentHolder;
    [SerializeField] GameObject stockAttachmentHolder;
    [SerializeField] GameObject muzzleAttachmentHolder;

    private void Awake()
    {
        instance = this;
        workbenchUI = transform.GetChild(0).gameObject;
    }

    #region UI Initialisation
    public void InitialiseWorkbenchUI()
    {
        OpenUI();
        AssignWeaponsToSlots();
        UpdatePointsCounter();
    }

    void AssignWeaponsToSlots()
    {
        if(WeaponSwapping.instance.currentPrimary1Weapon != null)
        {
            primarySlot1WeaponHolder.InitialiseWeaponHolder(WeaponSwapping.instance.currentPrimary1Weapon);
        }
        if(WeaponSwapping.instance.currentPrimary2Weapon != null)
        {
            primarySlot2WeaponHolder.InitialiseWeaponHolder(WeaponSwapping.instance.currentPrimary2Weapon);
        }
        if (WeaponSwapping.instance.currentSecondaryWeapon != null)
        {
            SecondarySlotWeaponHolder.InitialiseWeaponHolder(WeaponSwapping.instance.currentSecondaryWeapon);
        }
    }

    void UpdatePointsCounter()
    {
        pointsText.text = "£" + PointsManager.instance.currentPoints;
    }
    #endregion

    public void OpenUI()
    {
        workbenchUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void CloseUI()
    {
        workbenchUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisplayWeapon(Weapon weaponToDisplay)
    {
        if(currentlyDisplayedWeaponObj != null)
            Destroy(currentlyDisplayedWeaponObj);

        currentlyDisplayedWeaponObj = Instantiate(weaponToDisplay.UIObj, weaponModelSpawnPos);
        DisplayWeaponStats(weaponToDisplay);
        DisplayAttachmentPoints(weaponToDisplay);
    }

    void DisplayWeaponStats(Weapon weaponToDisplay)
    {
        weaponStatsPanel.SetActive(true);

        wepNameText.text = weaponToDisplay.name;
        wepDmgText.text = "Damage: " + weaponToDisplay.damage;
        wepAccText.text = "Accuracy: " + weaponToDisplay.maxSpreadDeviationAngle;
        wepFireRateText.text = "Fire Rate: " + weaponToDisplay.fireRate;
        wepReloadSpeedText.text = "Reload Speed: " + weaponToDisplay.reloadSpeed;
        wepMagSizeText.text = "Mag Size: " + weaponToDisplay.magSize;
        wepReserveAmmoText.text = "Max Reserve Ammo: " + weaponToDisplay.maxReserveAmmo;
    }

    void DisplayAttachmentPoints(Weapon weaponToDisplay)
    {
        AttachmentsPanel.SetActive(true);

        if(weaponToDisplay.canUseMagAttachment)
        {
            magAttachmentHolder.SetActive(true);
        }
        if (weaponToDisplay.canUseMuzzleAttachment)
        {
            muzzleAttachmentHolder.SetActive(true);
        }
        if (weaponToDisplay.canUseSightAttachment)
        {
            sightAttachmentHolder.SetActive(true);
        }
        if (weaponToDisplay.canUseStockAttachment)
        {
            stockAttachmentHolder.SetActive(true);
        }
    }
}
