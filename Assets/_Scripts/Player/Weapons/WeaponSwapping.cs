using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapping : MonoBehaviour
{
    public static WeaponSwapping instance;

    [SerializeField] GameObject primaryWeapon1Holder, primaryWeapon2Holder, secondaryWeaponHolder;

    public Weapon currentPrimary1Weapon, currentPrimary2Weapon, currentSecondaryWeapon;
    public GameObject currentPrimary1WeaponObj, currentPrimary2WeaponObj, currentSecondaryWeaponObj;

    public Weapon startingSecondaryWeapon;

    public List<GameObject> currentlyEquippedWeaponsList = new List<GameObject>();

    //1 == primary1, 2 == primary2, 3 == secondary
    public int currentlyEquippedWeaponSlot;
    public Weapon currentlyEquippedWeapon;
    public GameObject currentlyEquippedWeaponObj;

    [Header("Weapon Inventory")]
    public GameObject weaponInventoryDisplayUI;
    [SerializeField] float displayLength;
    Coroutine displayCounter;
    bool isWeaponInventoryDisplayOpen = false;

    [SerializeField] Vector3 weaponSpawnDefaultPosition;

    public static bool canSwapWeapon;

    public static event Action<GameObject> onWeaponSwapped;

    private void OnEnable()
    {
        MaxAmmo.onMaxAmmoGrabbed += RefillWeaponAmmunition;
    }

    private void OnDisable()
    {
        MaxAmmo.onMaxAmmoGrabbed -= RefillWeaponAmmunition;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        weaponInventoryDisplayUI.SetActive(false);

        canSwapWeapon = true;
        SpawnNewWeapon(startingSecondaryWeapon, 3);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowWeaponInventory();
            if (currentlyEquippedWeaponSlot != 1 && currentPrimary1Weapon)
                SwapToWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowWeaponInventory();
            if (currentlyEquippedWeaponSlot != 2 && currentPrimary2Weapon)
                SwapToWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowWeaponInventory();
            if (currentlyEquippedWeaponSlot != 3 && currentSecondaryWeapon)
                SwapToWeapon(3);
        }
    }

    public void PickUpWeapon(Weapon weaponToPickup)
    {

        if (weaponToPickup.weaponSlotType == Weapon.WeaponSlotType.primary)
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
        else if (weaponToPickup.weaponSlotType == Weapon.WeaponSlotType.secondary)
        {

            if (!currentSecondaryWeapon)
            {
                SpawnNewWeapon(weaponToPickup, 3);
            }
            else if (currentSecondaryWeapon)
            {
                ExchangeSecondaryWeapon(weaponToPickup);
            }
        }

        ShowWeaponInventory();
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
                currentlyEquippedWeaponsList.Remove(currentPrimary1WeaponObj);
                break;
            case 2:
                Destroy(currentPrimary2WeaponObj);
                currentlyEquippedWeaponsList.Remove(currentPrimary2WeaponObj);
                break;
            case 3:
                Destroy(currentSecondaryWeaponObj);
                currentlyEquippedWeaponsList.Remove(currentSecondaryWeaponObj);
                break;
        }
    }

    void SpawnNewWeapon(Weapon weaponToSpawn, int weaponSlot)
    {
        if (currentlyEquippedWeaponObj)
        {
            if(TryGetComponent<WeaponShooting>(out WeaponShooting weaponShooting))
            {
                if (weaponShooting.isReloading)
                    weaponShooting.CancelReload();

                if (weaponShooting.isAiming)
                    weaponShooting.StopADS();
            }

            if(currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().isReloading)
                currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().CancelReload();

            if(currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().isAiming)
                currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().StopADS();

        }

        DeactivateWeapons();
        GameObject clone = Instantiate(weaponToSpawn.weaponObj);
        //PlayerMovement.instance.animator = clone.GetComponent<Animator>();
        switch (weaponSlot)
        {
            case 1:
                primaryWeapon1Holder.SetActive(true);
                currentPrimary1WeaponObj = clone;
                currentPrimary1Weapon = weaponToSpawn;
                clone.transform.SetParent(primaryWeapon1Holder.transform);
                break;
            case 2:
                primaryWeapon2Holder.SetActive(true);
                currentPrimary2WeaponObj = clone;
                currentPrimary2Weapon = weaponToSpawn;
                clone.transform.SetParent(primaryWeapon2Holder.transform);
                break;
            case 3:
                secondaryWeaponHolder.SetActive(true);
                currentSecondaryWeaponObj = clone;
                currentSecondaryWeapon = weaponToSpawn;
                clone.transform.SetParent(secondaryWeaponHolder.transform);
                break;
        }

        if (weaponToSpawn.name == "Python")
            clone.transform.localPosition = new Vector3(0, -1.6f, 0);
        else
            clone.transform.localPosition = weaponSpawnDefaultPosition;

        clone.transform.localRotation = new Quaternion(0, 0, 0, 0);
        clone.GetComponent<WeaponShooting>().InitialiseWeapon(weaponToSpawn);
        currentlyEquippedWeaponsList.Add(clone);
        currentlyEquippedWeaponSlot = weaponSlot;
        currentlyEquippedWeapon = weaponToSpawn;
        currentlyEquippedWeaponObj = clone;
    }

    void SwapToWeapon(int weaponSlotToSwapTo)
    {
        if (currentlyEquippedWeaponObj && currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().isReloading)
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
                    //PlayerMovement.instance.animator = currentlyEquippedWeaponObj.GetComponent<Animator>();
                }
                break;
            case 2:
                if (currentPrimary2Weapon)
                {
                    currentlyEquippedWeaponSlot = 2;
                    currentlyEquippedWeapon = currentPrimary2Weapon;
                    currentlyEquippedWeaponObj = currentPrimary2WeaponObj;
                    primaryWeapon2Holder.SetActive(true);
                    //PlayerMovement.instance.animator = currentlyEquippedWeaponObj.GetComponent<Animator>();
                }
                break;
            case 3:
                if (currentSecondaryWeapon)
                {
                    currentlyEquippedWeaponSlot = 3;
                    currentlyEquippedWeapon = currentSecondaryWeapon;
                    currentlyEquippedWeaponObj = currentSecondaryWeaponObj;
                    secondaryWeaponHolder.SetActive(true);
                    //PlayerMovement.instance.animator = currentlyEquippedWeaponObj.GetComponent<Animator>();
                }
                break;
        }
        weaponInventoryDisplayUI.GetComponentInParent<WeaponInventoryDisplay>().UpdateSelectedSlot(currentlyEquippedWeaponSlot);
        currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().CheckInstantKillStatus();
        currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().CheckBottomlessClipStatus();
        currentlyEquippedWeaponObj.GetComponent<WeaponShooting>().ApplyUpgradeModifiers();
        onWeaponSwapped?.Invoke(currentlyEquippedWeaponObj);
    }

    void DeactivateWeapons()
    {
        primaryWeapon1Holder.SetActive(false);
        primaryWeapon2Holder.SetActive(false);
        secondaryWeaponHolder.SetActive(false);
    }

    public void TemporarilyDeactivateWeapons(GameObject meleeObjHolder, float reactivationTime)
    {
        DeactivateWeapons();
        StartCoroutine(ReactivateWeapons(meleeObjHolder, reactivationTime));
    }

    IEnumerator ReactivateWeapons(GameObject grenadeObjHolder, float reactivationTime)
    {
        yield return new WaitForSeconds(reactivationTime);
        grenadeObjHolder.SetActive(false);
        switch (currentlyEquippedWeaponSlot)
        {
            case 1:
                primaryWeapon1Holder.SetActive(true);
                currentPrimary1WeaponObj.GetComponent<WeaponShooting>().CheckInstantKillStatus();
                currentPrimary1WeaponObj.GetComponent<WeaponShooting>().CheckBottomlessClipStatus();
                break;
            case 2:
                primaryWeapon2Holder.SetActive(true);
                currentPrimary2WeaponObj.GetComponent<WeaponShooting>().CheckInstantKillStatus();
                currentPrimary2WeaponObj.GetComponent<WeaponShooting>().CheckBottomlessClipStatus();
                break;
            case 3:
                secondaryWeaponHolder.SetActive(true);
                currentSecondaryWeaponObj.GetComponent<WeaponShooting>().CheckInstantKillStatus();
                currentSecondaryWeaponObj.GetComponent<WeaponShooting>().CheckBottomlessClipStatus();
                break;
        }
    }

    public void RefillWeaponAmmunition()
    {
        if (currentPrimary1WeaponObj != null)
            currentPrimary1WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();

        if (currentPrimary2WeaponObj != null)
            currentPrimary2WeaponObj.GetComponent<WeaponShooting>().RefillAmmo();

        if (currentSecondaryWeaponObj != null)
            currentSecondaryWeaponObj.GetComponent<WeaponShooting>().RefillAmmo();
    }

    void ShowWeaponInventory()
    {
        weaponInventoryDisplayUI.SetActive(true);
        isWeaponInventoryDisplayOpen = true;
        weaponInventoryDisplayUI.GetComponentInParent<WeaponInventoryDisplay>().Init(currentlyEquippedWeaponSlot, currentSecondaryWeapon, currentPrimary1Weapon != null ? currentPrimary1Weapon : null, currentPrimary2Weapon != null ? currentPrimary2Weapon : null);
        weaponInventoryDisplayUI.transform.DOScaleY(1, .2f).OnComplete(() =>
        {
            if(displayCounter != null)
            {
                StopCoroutine(displayCounter);
            }
            displayCounter = StartCoroutine(StartDisplayCountdown());
        });

    }

    void HideWeaponInventory()
    {
        isWeaponInventoryDisplayOpen = false;
        weaponInventoryDisplayUI.transform.DOScaleY(0, .2f).OnComplete(() =>
        {
            if(isWeaponInventoryDisplayOpen == false)
            {
                weaponInventoryDisplayUI.SetActive(false);
            }
        });
    }


    IEnumerator StartDisplayCountdown()
    {
        yield return new WaitForSeconds(displayLength);
        HideWeaponInventory();

    }

}
