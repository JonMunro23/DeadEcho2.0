using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class RecoilManager : MonoBehaviour
{
    WeaponSwapping weaponSwapping;
    Weapon currentWeapon;

    Vector3 currentRotation;
    Vector3 targetRotation;

    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;

    private void OnEnable()
    {
        WeaponShooting.onWeaponFired += RecoilFire;
    }

    private void OnDisable()
    {
        WeaponShooting.onWeaponFired -= RecoilFire;
    }

    private void Awake()
    {
        weaponSwapping = GetComponentInChildren<WeaponSwapping>();
    }

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(bool isAiming)
    {
        currentWeapon = weaponSwapping.currentlyEquippedWeapon;

        if (isAiming)
        {
            targetRotation += new Vector3(currentWeapon.ADSRecoilX, Random.Range(-currentWeapon.ADSRecoilY, currentWeapon.ADSRecoilY), Random.Range(-currentWeapon.ADSRecoilZ, currentWeapon.ADSRecoilZ));
            return;
        }

        targetRotation += new Vector3(currentWeapon.recoilX, Random.Range(-currentWeapon.recoilY, currentWeapon.recoilY), Random.Range(-currentWeapon.recoilZ, currentWeapon.recoilZ));
        
    }
}
