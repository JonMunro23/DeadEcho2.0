using UnityEngine;

public class HandTargetController : MonoBehaviour
{
    [SerializeField] Transform leftHandTarget, rightHandTarget, leftGunTarget, rightGunTarget;

    WeaponShooting currentWeaponShootingScript;

    private void OnEnable()
    {
        WeaponSwapping.onWeaponSwapped += UpdateHandTargets;
    }

    private void OnDisable()
    {
        WeaponSwapping.onWeaponSwapped -= UpdateHandTargets;
    }

    // Update is called once per frame
    void Update()
    {
        leftHandTarget.position = leftGunTarget.position;
        leftHandTarget.rotation = leftGunTarget.rotation;

        rightHandTarget.position = rightGunTarget.position;
        rightHandTarget.rotation = rightGunTarget.rotation;
    }

    void UpdateHandTargets(GameObject weaponObj)
    {
        if(weaponObj.TryGetComponent<WeaponShooting>(out WeaponShooting weaponShooting))
        {
            currentWeaponShootingScript = weaponShooting;

            rightGunTarget = weaponShooting.weaponRightHandTarget;
            leftGunTarget = weaponShooting.weaponLeftHandTarget;
        }
    }

    void FinishedReloadAnimation()
    {
        currentWeaponShootingScript.ReloadFinished();
    }
}
