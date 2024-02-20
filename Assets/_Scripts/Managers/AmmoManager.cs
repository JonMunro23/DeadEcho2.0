using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    TMP_Text currentlyEquippedWeaponLoadedAmmoText, currentlyEquippedWeaponReserveAmmoText;

    private void Awake()
    {
        currentlyEquippedWeaponLoadedAmmoText = GameObject.FindGameObjectWithTag("LoadedAmmoText").GetComponent<TMP_Text>();
        currentlyEquippedWeaponReserveAmmoText = GameObject.FindGameObjectWithTag("ReserveAmmoText").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        WeaponShooting.onAmmoUpdated += UpdateAmmoHUD;
        WeaponSwapping.onWeaponSwapped += GetAmmoOfSwappedWeapon;        
    }

    private void OnDisable()
    {
        WeaponShooting.onAmmoUpdated -= UpdateAmmoHUD;
        WeaponSwapping.onWeaponSwapped -= GetAmmoOfSwappedWeapon;
    }

    void GetAmmoOfSwappedWeapon(GameObject _weaponSwappedTo)
    {
       WeaponShooting swappedWeaponShootingScript = _weaponSwappedTo.GetComponent<WeaponShooting>();
       UpdateAmmoHUD(swappedWeaponShootingScript.currentLoadedAmmo, swappedWeaponShootingScript.currentReserveAmmo);
    }

    public void UpdateAmmoHUD(int _currentLoadedAmmo, int _currentReserveAmmo)
    {
        currentlyEquippedWeaponLoadedAmmoText.text = _currentLoadedAmmo.ToString();
        currentlyEquippedWeaponReserveAmmoText.text = _currentReserveAmmo.ToString();
    }
}
