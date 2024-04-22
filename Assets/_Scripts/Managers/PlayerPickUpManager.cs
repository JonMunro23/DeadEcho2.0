using TMPro;
using UnityEngine;
using HighlightPlus;

public class PlayerPickUpManager : MonoBehaviour
{
    TMP_Text purchasePickupText;

    [Header("Keybinds")]
    [SerializeField] KeyCode purchaseWeaponKey = KeyCode.E;

    [SerializeField] AudioSource purchaseWeaponAudioSource;
    [SerializeField] int costOfAmmo;
    WeaponData weaponToPurchase;
    bool canBuy, isBuyingWeapon, isBuyingAmmo;

    //HighlightEffect wallBuyHighlightEffect;
    //Color originalHighlightEffectColor;

    private void Awake()
    {
        purchasePickupText = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TMP_Text>();
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
                        if (weaponToPurchase.name != WeaponSwapping.instance.currentlyEquippedWeapon.weaponData.name)
                            WeaponSwapping.instance.SpawnNewWeapon(weaponToPurchase);
                        else
                            return;

                        Purchase(weaponToPurchase.cost);
                    }
                    isBuyingWeapon = false;
                }
                else if(isBuyingAmmo)
                {
                    if (PointsManager.instance.currentPoints >= costOfAmmo)
                    {
                        if(WeaponSwapping.instance.currentPrimary1Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary1Weapon.weaponData.name)
                        {
                            if (!WeaponSwapping.instance.currentPrimary1WeaponObj.GetComponent<WeaponShooting>().IsAmmoFull())
                            {
                                WeaponSwapping.instance.currentPrimary1WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                                Purchase(costOfAmmo);
                            }
                            else
                                return;
                        }
                        else if (WeaponSwapping.instance.currentPrimary2Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary2Weapon.weaponData.name)
                        {
                            if (!WeaponSwapping.instance.currentPrimary2WeaponObj.GetComponent<WeaponShooting>().IsAmmoFull())
                            {
                                WeaponSwapping.instance.currentPrimary2WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                                Purchase(costOfAmmo);
                            }
                            else
                                return;
                        }
                        else if (WeaponSwapping.instance.currentSecondaryWeapon && weaponToPurchase.name == WeaponSwapping.instance.currentSecondaryWeapon.weaponData.name)
                        {
                            if (!WeaponSwapping.instance.currentSecondaryWeaponObj.GetComponent<WeaponShooting>().IsAmmoFull())
                            {
                                WeaponSwapping.instance.currentSecondaryWeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
                                Purchase(costOfAmmo);
                            }
                            else
                                return;
                        }
                    }
                    isBuyingAmmo = false;
                }
            }
        }
    }

    void Purchase(int cost)
    {
        purchaseWeaponAudioSource.PlayOneShot(purchaseWeaponAudioSource.clip);
        PointsManager.instance.RemovePoints(cost);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("WallBuy"))
    //    {
    //        canBuy = true;
    //        WallBuy wallBuy = other.GetComponent<WallBuy>();
    //        weaponToPurchase = wallBuy.weapon;
    //        if (WeaponSwapping.instance.currentPrimary1Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary1Weapon.name || WeaponSwapping.instance.currentPrimary2Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary2Weapon.name || WeaponSwapping.instance.currentSecondaryWeapon && weaponToPurchase.name == WeaponSwapping.instance.currentSecondaryWeapon.name)
    //        {
    //            purchasePickupText.text = "Press " + purchaseWeaponKey.ToString() + " to purchase ammo for £" + costOfAmmo;
    //            isBuyingWeapon = false;
    //            isBuyingAmmo = true;
    //        }
    //        else
    //        {
    //            purchasePickupText.text = "Press " + purchaseWeaponKey.ToString() + " to purchase the " + wallBuy.weaponName + " for £" + wallBuy.weaponCost;
    //            isBuyingAmmo = false;
    //            isBuyingWeapon = true;
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("WallBuy"))
        {
            //wallBuyHighlightEffect = other.GetComponentInChildren<HighlightEffect>();
            //originalHighlightEffectColor = wallBuyHighlightEffect.outlineColor;
            canBuy = true;
            WallBuy wallBuy = other.GetComponent<WallBuy>();
            weaponToPurchase = wallBuy.weapon;

            //change highlight outline color
            if (PointsManager.instance.currentPoints >= weaponToPurchase.cost && PointsManager.instance.currentPoints >= costOfAmmo)
            {
                //wallBuyHighlightEffect.outlineColor = Color.green;
            }
            else if (PointsManager.instance.currentPoints < weaponToPurchase.cost && PointsManager.instance.currentPoints < costOfAmmo)
            {
                //wallBuyHighlightEffect.outlineColor = Color.red;
            }                

            if (WeaponSwapping.instance.currentPrimary1Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary1Weapon.weaponData.name || WeaponSwapping.instance.currentPrimary2Weapon && weaponToPurchase.name == WeaponSwapping.instance.currentPrimary2Weapon.weaponData.name || WeaponSwapping.instance.currentSecondaryWeapon && weaponToPurchase.name == WeaponSwapping.instance.currentSecondaryWeapon.weaponData.name)
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

    private void OnTriggerExit(Collider other)
    {
        canBuy = false;
        isBuyingAmmo = false;
        isBuyingWeapon = false;
        purchasePickupText.text = "";
        //if(wallBuyHighlightEffect)
        //    wallBuyHighlightEffect.outlineColor = originalHighlightEffectColor;
    }


}
