using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPickUpManager : MonoBehaviour
{
    [SerializeField] TMP_Text purchasePickupText;

    [Header("Keybinds")]
    [SerializeField] KeyCode purchaseWeaponKey = KeyCode.E;

    [SerializeField] AudioSource purchaseWeaponAudioSource;
    [SerializeField] int costOfAmmo;
    Weapon weaponToPurchase;
    bool canBuy, isBuyingWeapon, isBuyingAmmo;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canBuy)
        {
            if (Input.GetKeyDown(purchaseWeaponKey))
            {
                if(isBuyingWeapon)
                {
                    if (PointsManager.instance.currentPoints >= weaponToPurchase.cost)
                    {
                        if (weaponToPurchase.name != WeaponSwapping.instance.currentlyEquippedWeapon.name)
                            WeaponSwapping.instance.PickUpWeapon(weaponToPurchase);
                        else
                            return;
                        purchaseWeaponAudioSource.PlayOneShot(purchaseWeaponAudioSource.clip);
                        PointsManager.instance.RemovePoints(weaponToPurchase.cost);
                    }
                    isBuyingWeapon = false;
                }
                else if(isBuyingAmmo)
                {
                    if (PointsManager.instance.currentPoints >= costOfAmmo)
                    {
                        if(weaponToPurchase.name == WeaponSwapping.instance.currentPrimary1Weapon.name)
                        {
                            WeaponSwapping.instance.currentPrimary1WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                        }
                        else if (weaponToPurchase.name == WeaponSwapping.instance.currentPrimary2Weapon.name)
                        {
                            WeaponSwapping.instance.currentPrimary2WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                        }
                        else if (weaponToPurchase.name == WeaponSwapping.instance.currentSecondaryWeapon.name)
                        {
                            WeaponSwapping.instance.currentSecondaryWeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                        }
                        purchaseWeaponAudioSource.PlayOneShot(purchaseWeaponAudioSource.clip);
                        PointsManager.instance.RemovePoints(costOfAmmo);
                    }
                    isBuyingAmmo = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WallBuy"))
        {
            canBuy = true;
            WallBuy wallBuy = other.GetComponent<WallBuy>();
            weaponToPurchase = wallBuy.weapon;
            if (WeaponSwapping.instance.currentPrimary1Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary1Weapon.name || WeaponSwapping.instance.currentPrimary2Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary2Weapon.name || WeaponSwapping.instance.currentSecondaryWeapon && weaponToPurchase.name == WeaponSwapping.instance.currentSecondaryWeapon.name)
            {
                purchasePickupText.text = "Press " + purchaseWeaponKey.ToString() + " to purchase ammo for £" + costOfAmmo;
                isBuyingWeapon = false;
                isBuyingAmmo = true;
            }
            else
            {
                purchasePickupText.text = "Press " + purchaseWeaponKey.ToString() + " to purchase the " + wallBuy.weaponName + " for £" + wallBuy.weaponCost;
                isBuyingAmmo = false;
                isBuyingWeapon = true;
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{

    //}

    private void OnTriggerExit(Collider other)
    {
        canBuy = false;
        isBuyingAmmo = false;
        isBuyingWeapon = false;
        purchasePickupText.text = "";
    }


}
