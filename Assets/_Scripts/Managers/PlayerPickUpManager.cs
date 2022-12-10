using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPickUpManager : MonoBehaviour
{
    [SerializeField] TMP_Text purchasePickupText;

    [Header("Keybinds")]
    [SerializeField] KeyCode purchaseWeaponKey = KeyCode.E;

    Weapon weaponToPurchase;
    bool canPurchaseWeapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canPurchaseWeapon)
        {
            if (Input.GetKeyDown(purchaseWeaponKey))
            {
                WeaponSwapping.instance.PickUpWeapon(weaponToPurchase);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WallBuy"))
        {
            canPurchaseWeapon = true;
            WallBuy wallBuy = other.GetComponent<WallBuy>();
            purchasePickupText.text = "Press " + purchaseWeaponKey.ToString() + "To purchase the " + wallBuy.weaponName + " for £" + wallBuy.weaponCost;
            weaponToPurchase = wallBuy.weapon;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{

    //}

    private void OnTriggerExit(Collider other)
    {
        canPurchaseWeapon = false;
        purchasePickupText.text = "";
    }


}
