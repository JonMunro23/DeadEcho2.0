using UnityEngine;

public class RecoilManager : MonoBehaviour
{
    WeaponSwapping weaponSwapping;
    WeaponData currentWeapon;

    Vector3 currentRotation;
    Quaternion memeeRot;
    Vector3 targetRotation;

    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;

    float yRotation;
    float xRotation;

    [SerializeField]
    Transform orientation;

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
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * (20 * 10);
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * (20 * 10);

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        memeeRot = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);




        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation) * memeeRot;



    }

    public void RecoilFire(bool isAiming)
    {
        currentWeapon = weaponSwapping.currentlyEquippedWeapon.weaponData;

        if (isAiming)
        {
            targetRotation += new Vector3(currentWeapon.ADSRecoilX, Random.Range(-currentWeapon.ADSRecoilY, currentWeapon.ADSRecoilY), Random.Range(-currentWeapon.ADSRecoilZ, currentWeapon.ADSRecoilZ));
            return;
        }

        targetRotation += new Vector3(currentWeapon.recoilX, Random.Range(-currentWeapon.recoilY, currentWeapon.recoilY), Random.Range(-currentWeapon.recoilZ, currentWeapon.recoilZ));
        
    }
}
