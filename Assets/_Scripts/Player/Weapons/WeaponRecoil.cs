using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField]
    float zRecoilAmount, aimingZRecoilAmount, xRotAmount, aimingXRotAmount, yRotAmount, aimingYRotAmount, zRotAmount, aimingZRotAmount;

    private void OnEnable()
    {
        WeaponShooting.onWeaponFired += Recoil;
    }

    private void OnDisable()
    {
        WeaponShooting.onWeaponFired -= Recoil;
    }

    void Recoil(bool isAiming)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (!isAiming ? zRecoilAmount : aimingZRecoilAmount));
        transform.localRotation = Quaternion.Euler(transform.localRotation.x + (!isAiming ? xRotAmount : aimingXRotAmount), transform.localRotation.y + Random.Range(-(!isAiming ? yRotAmount : aimingYRotAmount), (!isAiming ? yRotAmount : aimingYRotAmount)), transform.localRotation.z + Random.Range(-(!isAiming ? zRotAmount : aimingZRotAmount), (!isAiming ? zRotAmount : aimingZRecoilAmount)));
    }
}
