using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapping : MonoBehaviour
{
    public static WeaponSwapping instance;
    public WeaponData[] startingWeapons;
    [Space]
    [Header("Current Weapon Data")]
    public WeaponShooting currentlyEquippedWeapon;
    public GameObject currentlyEquippedWeaponObj;
    [Header("Primary 1 Data")]
    public WeaponShooting currentPrimary1Weapon;
    public GameObject currentPrimary1WeaponObj;
    [Header("Primary 2 Data")]
    public WeaponShooting currentPrimary2Weapon;
    public GameObject currentPrimary2WeaponObj;
    [Header("Secondary Data")]
    public WeaponShooting currentSecondaryWeapon;
    public GameObject currentSecondaryWeaponObj;
    [Header("PowerUp Weapon Data")]
    public WeaponShooting currentPowerUpWeapon;
    public GameObject currentPowerUpWeaponObj;
    [Space]
    public int currentActiveSlot;

    public List<WeaponShooting> currentlyEquippedWeaponsList = new List<WeaponShooting>();

    public Transform GunBone, inactiveWeaponParent;
    public Animator FPSArmsAnimator;

    [Header("Weapon Inventory UI")]
    public GameObject weaponInventoryDisplayUI;
    [SerializeField] float displayLength;
    Coroutine displayCounter;
    bool isWeaponInventoryDisplayOpen = false;

    public static bool canSwapWeapon;

    public static event Action<GameObject> onWeaponSwapped;

    //private void OnEnable()
    //{
    //    MaxAmmo.onMaxAmmoGrabbed += RefillWeaponAmmunition;
    //}

    //private void OnDisable()
    //{
    //    MaxAmmo.onMaxAmmoGrabbed -= RefillWeaponAmmunition;
    //}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        weaponInventoryDisplayUI.SetActive(false);
        canSwapWeapon = true;

        foreach (WeaponData weapon in startingWeapons)
        {
            SpawnNewWeapon(weapon);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowWeaponInventory();
            if (currentPrimary1Weapon != null)
                SwapToWeapon(1);
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowWeaponInventory();
            if(currentPrimary2Weapon != null)
                SwapToWeapon(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowWeaponInventory();
            if (currentSecondaryWeapon != null)
                SwapToWeapon(3);

        }
    }

    void SwapToWeapon(int weaponSlot)
    {
        if (currentlyEquippedWeapon)
            DetachWeaponFromGunBone(currentlyEquippedWeapon);

        SetWeaponActive(weaponSlot);
    }

    public void SpawnNewWeapon(WeaponData weaponToSpawn)
    {
        if (currentlyEquippedWeapon)
            DetachWeaponFromGunBone(currentlyEquippedWeapon);

        GameObject clone = Instantiate(weaponToSpawn.weaponObj);
        currentlyEquippedWeaponObj = clone;
        currentlyEquippedWeapon = clone.GetComponent<WeaponShooting>();
        currentlyEquippedWeaponsList.Add(currentlyEquippedWeapon);
        currentlyEquippedWeapon.InitialiseWeapon(weaponToSpawn, FPSArmsAnimator);

        AssignWeaponToSlot(currentlyEquippedWeapon);
        SetWeaponActive(currentlyEquippedWeapon.weaponSlot);
    }

    void SetCurrentlyEquippedWeapon(WeaponShooting weaponToEquip)
    {
        currentlyEquippedWeapon = weaponToEquip;
        currentlyEquippedWeaponObj = weaponToEquip.gameObject;
    }

    void SetWeaponActive(int slot)
    {
        GameObject weaponToSetActive = null;
        switch (slot)
        {
            case 1:
                AttachWeaponToGunBone(currentPrimary1WeaponObj.transform);
                SetCurrentlyEquippedWeapon(currentPrimary1Weapon);
                currentPrimary1WeaponObj.transform.localPosition = currentPrimary1Weapon.weaponData.weaponSpawnPos;
                FPSArmsAnimator.runtimeAnimatorController = currentPrimary1Weapon.weaponData.armsController;
                weaponToSetActive = currentPrimary1WeaponObj;
                break;
            case 2:
                AttachWeaponToGunBone(currentPrimary2WeaponObj.transform);
                SetCurrentlyEquippedWeapon(currentPrimary2Weapon);
                currentPrimary2WeaponObj.transform.localPosition = currentPrimary2Weapon.weaponData.weaponSpawnPos;
                FPSArmsAnimator.runtimeAnimatorController = currentPrimary2Weapon.weaponData.armsController;
                weaponToSetActive = currentPrimary2WeaponObj;

                break;
            case 3:
                AttachWeaponToGunBone(currentSecondaryWeaponObj.transform);
                SetCurrentlyEquippedWeapon(currentSecondaryWeapon);
                currentSecondaryWeaponObj.transform.localPosition = currentSecondaryWeapon.weaponData.weaponSpawnPos;
                FPSArmsAnimator.runtimeAnimatorController = currentSecondaryWeapon.weaponData.armsController;
                weaponToSetActive = currentSecondaryWeaponObj;
                break;
            case 4:
                AttachWeaponToGunBone(currentPowerUpWeaponObj.transform);
                SetCurrentlyEquippedWeapon(currentPowerUpWeapon);
                currentPowerUpWeaponObj.transform.localPosition = currentPowerUpWeapon.weaponData.weaponSpawnPos;
                FPSArmsAnimator.runtimeAnimatorController = currentPowerUpWeapon.weaponData.armsController;
                weaponToSetActive = currentPowerUpWeaponObj;
                break;
        }
        onWeaponSwapped?.Invoke(weaponToSetActive);
    }

    void AssignWeaponToSlot(WeaponShooting weaponToAssign)
    {
        switch (weaponToAssign.weaponData.weaponSlotType)
        {
            case WeaponData.WeaponSlotType.primary:

                if(currentPrimary1Weapon != null && currentPrimary2Weapon != null)
                {
                    if(currentlyEquippedWeapon.weaponSlot == 2)
                    {
                        ExchangeWeaponInSlot(2, weaponToAssign);
                        return;
                    }

                    ExchangeWeaponInSlot(1, weaponToAssign);
                    return;
                }

                if (currentPrimary1Weapon == null)
                {
                    currentPrimary1Weapon = weaponToAssign;
                    currentPrimary1WeaponObj = weaponToAssign.gameObject;
                    currentPrimary1Weapon.weaponSlot = 1;
                }
                else if(currentPrimary2Weapon == null)
                {
                    currentPrimary2Weapon = weaponToAssign;
                    currentPrimary2WeaponObj = weaponToAssign.gameObject;
                    currentPrimary2Weapon.weaponSlot = 2;
                }

                break;
            case WeaponData.WeaponSlotType.secondary:

                if (currentSecondaryWeapon == null)
                {
                    currentSecondaryWeapon = weaponToAssign;
                    currentSecondaryWeaponObj = weaponToAssign.gameObject;
                    currentSecondaryWeapon.weaponSlot = 3;
                }
                else
                    ExchangeWeaponInSlot(3, weaponToAssign);

                break;
            case WeaponData.WeaponSlotType.powerUp:
                if (currentPowerUpWeapon == null)
                {
                    currentPowerUpWeapon = weaponToAssign;
                    currentPowerUpWeaponObj = weaponToAssign.gameObject;
                    currentPowerUpWeapon.weaponSlot = 4;
                }
                break;
        }
    }

    void ExchangeWeaponInSlot(int weaponSlot, WeaponShooting newWeapon)
    {
        switch (weaponSlot)
        {
            case 1:
                Destroy(currentPrimary1WeaponObj);
                currentlyEquippedWeaponsList.Remove(currentPrimary1Weapon);
                currentPrimary1Weapon = newWeapon;
                currentPrimary1WeaponObj = newWeapon.gameObject;
                currentPrimary1Weapon.weaponSlot = 1;
                break;
            case 2:
                Destroy(currentPrimary2WeaponObj);
                currentlyEquippedWeaponsList.Remove(currentPrimary2Weapon);
                currentPrimary2Weapon = newWeapon;
                currentPrimary2WeaponObj = newWeapon.gameObject;
                currentPrimary2Weapon.weaponSlot = 2;
                break;
            case 3:
                Destroy(currentSecondaryWeaponObj);
                currentlyEquippedWeaponsList.Remove(currentSecondaryWeapon);
                currentSecondaryWeapon = newWeapon;
                currentSecondaryWeaponObj = newWeapon.gameObject;
                currentSecondaryWeapon.weaponSlot = 3;
                break;
        }
    }

    void DetachWeaponFromGunBone(WeaponShooting weaponToDetach)
    {
        if (weaponToDetach.isReloading)
            weaponToDetach.CancelReload();

        if (weaponToDetach.isAiming)
            weaponToDetach.StopADS();

        Vector3 weaponScale = weaponToDetach.transform.localScale;
        weaponToDetach.transform.SetParent(inactiveWeaponParent);
        weaponToDetach.transform.localScale = weaponScale;
        ShowWeapon(weaponToDetach.gameObject, false);
    }

    void AttachWeaponToGunBone(Transform weaponToAttach)
    {
        Vector3 weaponScale = weaponToAttach.localScale;
        weaponToAttach.SetParent(GunBone);
        weaponToAttach.localScale = weaponScale;
        weaponToAttach.localRotation = new Quaternion(0, 0, 0, 0);

        ShowWeapon(weaponToAttach.gameObject, true);
    }

    void ShowWeapon(GameObject weaponToSet, bool isActive, bool affectArms = false)
    {
        if(affectArms)
            FPSArmsAnimator.gameObject.SetActive(isActive);

        weaponToSet.SetActive(isActive);
    }

    public void RefillWeaponAmmunition()
    {
        foreach (WeaponShooting weapon in currentlyEquippedWeaponsList)
        {
            weapon.RefillAmmo();
        }
    }

    public void GivePowerUpWeapon(WeaponData weaponToGive)
    {
        SpawnNewWeapon(weaponToGive);
    }

    public void RemovePowerUpWeapon()
    {
        if (currentPrimary1Weapon != null)
            SwapToWeapon(1);
        else if (currentPrimary2Weapon != null)
            SwapToWeapon(2);
        else 
            SwapToWeapon(3);
    }

    void ShowWeaponInventory()
    {
        weaponInventoryDisplayUI.SetActive(true);
        isWeaponInventoryDisplayOpen = true;
        //weaponInventoryDisplayUI.GetComponentInParent<WeaponInventoryDisplay>().Init(currentActiveSlot, currentSecondaryWeapon.weaponData, currentPrimary1Weapon != null ? currentPrimary1Weapon.weaponData : null, currentPrimary2Weapon != null ? currentPrimary2Weapon.weaponData : null);
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

    public void TemporarilyDeactivateWeapons(GameObject meleeObjHolder, float reactivationTime)
    {
        ShowWeapon(currentlyEquippedWeaponObj, false, true);
        StartCoroutine(ReactivateWeapons(meleeObjHolder, reactivationTime));
    }

    IEnumerator ReactivateWeapons(GameObject grenadeObjHolder, float reactivationTime)
    {
        yield return new WaitForSeconds(reactivationTime);
        grenadeObjHolder.SetActive(false);
        ShowWeapon(currentlyEquippedWeaponObj, true, true);
    }

}
