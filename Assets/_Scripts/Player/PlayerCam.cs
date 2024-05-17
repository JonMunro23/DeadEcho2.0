using System.Collections;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivity;

    [HideInInspector] public Transform orientation;

    float xRotation;
    float yRotation;
    float defaultFOV;
    public bool canLook = true;

    [SerializeField] Camera weaponCamera;
    [SerializeField]
    AnimationCurve ADSCurve;

    Coroutine cameraFOVLerpCoroutine;
    Coroutine weaponLocalPositionLerpCoroutine;

    [Header("Recoil")]
    [SerializeField] float recoilSnappiness;
    [SerializeField] float recoilReturnSpeed;
    Vector3 currentRecoilRot;
    Vector3 targetRecoilRotation;
    WeaponSwapping weaponSwapping;
    WeaponData currentWeapon;

    private void Awake()
    {
        //weaponCamera = GetComponentInChildren<Camera>();
        weaponSwapping = GetComponentInChildren<WeaponSwapping>();
        orientation = GameObject.FindGameObjectWithTag("PlayerOrientation").transform;
    }

    private void OnEnable()
    {
        WeaponShooting.onAimDownSights += ToggleAiming;
        WeaponShooting.onWeaponFired += RecoilFire;
        PlayerHealth.onDeath += DisableCameraLook;
        OptionsMenu.updateSettings += UpdateCameraFOV;
        OptionsMenu.updateSettings += UpdateMouseSensitivity;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultFOV = weaponCamera.fieldOfView;
    }

    private void OnDisable()
    {
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponShooting.onWeaponFired -= RecoilFire;
        PlayerHealth.onDeath -= DisableCameraLook;
        OptionsMenu.updateSettings -= UpdateCameraFOV;
        OptionsMenu.updateSettings -= UpdateMouseSensitivity;
    }

    void Update()
    {
        if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen)
        {
            if (canLook)
            {
                float mouseX = Input.GetAxis("Mouse X") * sensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

                yRotation += mouseX;
                xRotation -= mouseY;

                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                //targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
                //currentRecoilRot = Vector3.Slerp(currentRecoilRot, targetRecoilRotation, recoilSnappiness * Time.fixedDeltaTime);
            
                transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0) /** Quaternion.Euler(currentRecoilRot)*/;
                orientation.rotation = Quaternion.Euler(0, yRotation, 0);          

            }
        }
    }

    public void RecoilFire(bool isAiming)
    {
        //currentWeapon = weaponSwapping.currentlyEquippedWeapon.weaponData;

        //if (isAiming)
        //{
        //    targetRecoilRotation += new Vector3(currentWeapon.recoilData.cameraADSRotRecoil.x, Random.Range(-currentWeapon.recoilData.cameraADSRotRecoil.y, currentWeapon.recoilData.cameraADSRotRecoil.y), Random.Range(-currentWeapon.recoilData.cameraADSRotRecoil.z, currentWeapon.recoilData.cameraADSRotRecoil.z));
        //    return;
        //}

        //targetRecoilRotation += new Vector3(currentWeapon.recoilData.cameraRotRecoil.x, Random.Range(-currentWeapon.recoilData.cameraRotRecoil.y, currentWeapon.recoilData.cameraRotRecoil.y), Random.Range(-currentWeapon.recoilData.cameraRotRecoil.z, currentWeapon.recoilData.cameraRotRecoil.z));

    }

    public void ToggleAiming(bool _isAiming, WeaponShooting weaponToToggle)
    {
        if(!weaponToToggle.weaponData.isScoped)
        {
            if (_isAiming)
            {
                ChangeCameraFOV(weaponToToggle, defaultFOV);
            }
            else if (!_isAiming)
            {
                ChangeCameraFOV(weaponToToggle, weaponToToggle.weaponData.aimingFOV);
            }
        }
    }

    void ChangeCameraFOV(WeaponShooting weaponToToggle, float fov)
    {
        if (cameraFOVLerpCoroutine != null)
            StopCoroutine(cameraFOVLerpCoroutine);

        cameraFOVLerpCoroutine = StartCoroutine(LerpCameraFOV(fov, weaponToToggle.weaponData.timeToADS));
    }

    void DisableCameraLook()
    {
        canLook = false;
    }

    void UpdateCameraFOV(PlayerSettings playerSettings)
    {
        weaponCamera.fieldOfView = playerSettings.playerFov.currentValue;
        defaultFOV = playerSettings.playerFov.currentValue;
    }

    void UpdateMouseSensitivity(PlayerSettings playerSettings)
    {
        sensitivity = playerSettings.mouseSensitivity.currentValue;
    }

    IEnumerator LerpCameraFOV(float newFOV, float duration)
    {
        float _timeElapsed = 0;

        float reducedRange = Mathf.Abs(weaponCamera.fieldOfView - newFOV);
        float reducedDuration = duration * (reducedRange / newFOV);
        while (_timeElapsed < reducedDuration)
        {
            float t = _timeElapsed / reducedDuration;

            t = ADSCurve.Evaluate(t);

            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, newFOV, t);
            _timeElapsed += Time.deltaTime;

            yield return null;
        }
        weaponCamera.fieldOfView = newFOV;
    }
}
