using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowables : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] KeyCode throwEquipmentKey = KeyCode.G;
    
    [SerializeField] GameObject grenadeArmsHolder, grenadeObj;
    [SerializeField] Transform grenadeSpawnLocation;

    public bool canThrowGrenade;

    [SerializeField] int startingGrenadeCount;

    [HideInInspector] public int currentGrenadeCount;

    private void Start()
    {
        currentGrenadeCount = startingGrenadeCount;
        canThrowGrenade = true;
    }


    // Update is called once per frame
    void Update()
    {
        if(canThrowGrenade && currentGrenadeCount > 0)
        {
            if(Input.GetKeyDown(throwEquipmentKey))
            {
                canThrowGrenade = false;
                currentGrenadeCount--;
                WeaponShooting weaponShooting;
                weaponShooting = WeaponSwapping.instance.currentlyEquippedWeaponObj.GetComponent<WeaponShooting>();
                if (weaponShooting.isReloading)
                    weaponShooting.CancelReload();
                if (weaponShooting.isAiming)
                    weaponShooting.StopADS();
                grenadeArmsHolder.SetActive(true);
                WeaponSwapping.instance.DeactivateForThrowables(grenadeArmsHolder);
                StartCoroutine(ThrowGrenadeCooldown());
            }
        }
    }

    public void AddGrenades()
    {

    }

    IEnumerator ThrowGrenadeCooldown()
    {
        yield return new WaitForSeconds(2);
        canThrowGrenade = true;
    }

}
