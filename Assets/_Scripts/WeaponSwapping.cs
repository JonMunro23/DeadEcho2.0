using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapping : MonoBehaviour
{
    public static WeaponSwapping instance;

    [SerializeField] GameObject primaryWeapon1Holder, primaryWeapon2Holder, secondaryWeaponHolder;

    Weapon currentPrimary1Weapon, currentPrimary2Weapon, currentSecondaryWeapon;
    GameObject currentPrimary1WeaponObj, currentPrimary2WeaponObj, currentSecondaryWeaponObj;

    public Weapon startingPistol, tempPrimary1, tempPrimary2;

    //1 == primary1, 2 == primary2, 3 == secondary
    public int currentlyEquippedWeaponSlot;
    public Weapon currentlyEquippedWeapon;
    public GameObject currentlyEquippedWeaponObj;

    [SerializeField] Vector3 weaponSpawnDefaultPosition;

    public static bool canSwapWeapon;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canSwapWeapon = true;
        PickUpWeapon(startingPistol);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentlyEquippedWeaponSlot != 1 && currentPrimary1Weapon)
        {
            SwapToWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentlyEquippedWeaponSlot != 2 && currentPrimary2Weapon)
        {
            SwapToWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && currentlyEquippedWeaponSlot != 3 && currentSecondaryWeapon)
        {
            SwapToWeapon(3);
        }

        if(Input.GetKeyDown(KeyCode.F1))
        {
            PickUpWeapon(tempPrimary1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PickUpWeapon(tempPrimary2);
        }
    }

    public void PickUpWeapon(Weapon weaponToPickup)
    {
        if(weaponToPickup.weaponSlotType == Weapon.WeaponSlotType.primary)
        {
            if (!currentPrimary1Weapon)
            {
                SpawnNewWeapon(weaponToPickup, 1);
            }
            else if (!currentPrimary2Weapon)
            {
                SpawnNewWeapon(weaponToPickup, 2);
            }
            else if (currentPrimary1Weapon && currentPrimary2Weapon)
            {
                ExchangePrimaryWeapon(weaponToPickup);
            }
        }
        else if(weaponToPickup.weaponSlotType == Weapon.WeaponSlotType.secondary)
        {
            if (!currentSecondaryWeapon)
                SpawnNewWeapon(weaponToPickup, 3);
            else if(currentSecondaryWeapon)
            {
                ExchangeSecondaryWeapon(weaponToPickup);
            }
        }
    }


    void ExchangePrimaryWeapon(Weapon newWeapon)
    {
        switch (currentlyEquippedWeaponSlot)
        {
            case 1:
                DestroyPreviousWeapon(1);
                SpawnNewWeapon(newWeapon, 1);
                break;
            case 2:
                DestroyPreviousWeapon(2);
                SpawnNewWeapon(newWeapon, 2);
                break;
            case 3:
                DestroyPreviousWeapon(1);
                //Default swap first primary weapon if secondary is out
                SpawnNewWeapon(newWeapon, 1);
                break;
        }

    }
    void ExchangeSecondaryWeapon(Weapon newWeapon)
    {
        DestroyPreviousWeapon(3);
        SpawnNewWeapon(newWeapon, 3);
    }

    void DestroyPreviousWeapon(int weaponSlot)
    {
        switch (weaponSlot)
        {
            case 1:
                Destroy(currentPrimary1WeaponObj);
                break;
            case 2:
                Destroy(currentPrimary2WeaponObj);
                break;
            case 3:
                Destroy(currentSecondaryWeaponObj);
                break;
        }
    }

    void SpawnNewWeapon(Weapon weaponToSpawn, int weaponSlot)
    {
        if (currentlyEquippedWeaponObj && currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().isReloading)
        {
            currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().CancelReload();
        }
        DeactivateWeapons();
        GameObject clone = Instantiate(weaponToSpawn.weaponObj);
        PlayerMovement.instance.animator = clone.GetComponent<Animator>();
        switch(weaponSlot)
        {
            case 1:
                primaryWeapon1Holder.SetActive(true);
                currentPrimary1WeaponObj = clone;
                currentPrimary1Weapon = weaponToSpawn;
                clone.transform.SetParent(primaryWeapon1Holder.transform);
                UpdateAmmo(currentPrimary1WeaponObj);
                break;
            case 2:
                primaryWeapon2Holder.SetActive(true);
                currentPrimary2WeaponObj = clone;
                currentPrimary2Weapon = weaponToSpawn;
                clone.transform.SetParent(primaryWeapon2Holder.transform);
                UpdateAmmo(currentPrimary2WeaponObj);
                break;
            case 3:
                secondaryWeaponHolder.SetActive(true);
                currentSecondaryWeaponObj = clone;
                currentSecondaryWeapon = weaponToSpawn;
                clone.transform.SetParent(secondaryWeaponHolder.transform);
                UpdateAmmo(currentSecondaryWeaponObj);
                break;
        }
        clone.transform.localPosition = weaponSpawnDefaultPosition;
        clone.transform.localRotation = new Quaternion(0, 0, 0, 0);
        clone.GetComponent<WeaponShooting>().InitialiseNewWeaponObj(weaponToSpawn);
        currentlyEquippedWeaponSlot = weaponSlot;
        currentlyEquippedWeapon = weaponToSpawn;
        currentlyEquippedWeaponObj = clone;
    }

    void SwapToWeapon(int weaponSlotToSwapTo)
    {
        if(currentlyEquippedWeaponObj && currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().isReloading)
        {
            currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().CancelReload();
        }
        DeactivateWeapons();
        switch (weaponSlotToSwapTo)
        {
            case 1:
                if (currentPrimary1Weapon)
                {
                    currentlyEquippedWeaponSlot = 1;
                    currentlyEquippedWeapon = currentPrimary1Weapon;
                    currentlyEquippedWeaponObj = currentPrimary1WeaponObj;
                    primaryWeapon1Holder.SetActive(true);
                    PlayerMovement.instance.animator = currentPrimary1WeaponObj.GetComponent<Animator>();
                    UpdateAmmo(currentPrimary1WeaponObj);
                }
                break;
            case 2:
                if (currentPrimary2Weapon)
                {
                    currentlyEquippedWeaponSlot = 2;
                    currentlyEquippedWeapon = currentPrimary2Weapon;
                    currentlyEquippedWeaponObj = currentPrimary2WeaponObj;
                    primaryWeapon2Holder.SetActive(true);
                    PlayerMovement.instance.animator = currentPrimary2WeaponObj.GetComponent<Animator>();
                    UpdateAmmo(currentPrimary2WeaponObj);
                }
                break;
            case 3:
                if (currentSecondaryWeapon)
                {
                    currentlyEquippedWeaponSlot = 3;
                    currentlyEquippedWeapon = currentSecondaryWeapon;
                    currentlyEquippedWeaponObj = currentSecondaryWeaponObj;
                    secondaryWeaponHolder.SetActive(true);
                    PlayerMovement.instance.animator = currentSecondaryWeaponObj.GetComponent<Animator>();
                    UpdateAmmo(currentSecondaryWeaponObj);
                }
                break;
        }
    }

    void UpdateAmmo(GameObject weaponObj)
    {
        WeaponShooting weaponShooting = weaponObj.GetComponent<WeaponShooting>();
        AmmoManager.instance.UpdateAmmoHUD(weaponShooting.currentLoadedAmmo, weaponShooting.currentReserveAmmo);
    }

    void DeactivateWeapons()
    {
        primaryWeapon1Holder.SetActive(false);
        primaryWeapon2Holder.SetActive(false);
        secondaryWeaponHolder.SetActive(false);
    }
}
