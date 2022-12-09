using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager instance;

    [SerializeField]
    TMP_Text currentlyEquippedWeaponLoadedAmmoText, currentlyEquippedWeaponReserveAmmoText;

    private void Awake()
    {
        instance = this;
    }


    public void UpdateAmmoHUD(int currentLoadedAmmo, int currentReserveAmmo)
    {
        currentlyEquippedWeaponLoadedAmmoText.text = currentLoadedAmmo.ToString();
        currentlyEquippedWeaponReserveAmmoText.text = currentReserveAmmo.ToString();
    }
}
