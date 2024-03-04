using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivity;

    [HideInInspector] public Transform orientation;

    float xRotation;
    float yRotation;

    float defaultFOV;
    float timeToLeaveADS = 0.75f;

    public bool canLook = true;

    Camera weaponCamera;
    [SerializeField]
    AnimationCurve ADSCurve;

    Coroutine cameraFOVLerpCoroutine;
    Coroutine weaponLocalPositionLerpCoroutine;


    private void Awake()
    {
        weaponCamera = GetComponent<Camera>();
        orientation = GameObject.FindGameObjectWithTag("PlayerOrientation").transform;
    }

    private void OnEnable()
    {
        WeaponShooting.onAimDownSights += ToggleAiming;
        WeaponShooting.onWeaponFired += ApplyRecoil;
        PlayerHealth.onDeath += DisableCameraLook;
        OptionsMenu.updateSettings += UpdateCameraFOV;
        OptionsMenu.updateSettings += UpdateMouseSensitivity;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultFOV = weaponCamera.fieldOfView;
    }

    private void OnDisable()
    {
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponShooting.onWeaponFired -= ApplyRecoil;
        PlayerHealth.onDeath -= DisableCameraLook;
        OptionsMenu.updateSettings -= UpdateCameraFOV;
        OptionsMenu.updateSettings -= UpdateMouseSensitivity;
    }

    // Update is called once per frame
    void Update()
    {

        if (canLook)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * (sensitivity * 10);
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * (sensitivity * 10);

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    void ApplyRecoil(bool isAiming)
    {
        if (!isAiming)
        {

        }
    }

    
    public void ToggleAiming(bool _isAiming, WeaponShooting weaponToToggle)
    {
        if (_isAiming)
        {
            MoveCameraToDefaultPosition(weaponToToggle);
        }
        else if (!_isAiming)
        {
            MoveCameraToAimingPosition(weaponToToggle);
        }
    }

    void MoveCameraToAimingPosition(WeaponShooting weaponToToggle)
    {
        if (cameraFOVLerpCoroutine != null)
            StopCoroutine(cameraFOVLerpCoroutine);

        cameraFOVLerpCoroutine = StartCoroutine(LerpCameraFOV(weaponToToggle.equippedWeapon.aimingFOV, weaponToToggle.equippedWeapon.timeToADS));

        if (weaponLocalPositionLerpCoroutine != null)
            StopCoroutine(weaponLocalPositionLerpCoroutine);

        weaponLocalPositionLerpCoroutine = StartCoroutine(LerpWeaponLocalPosition(weaponToToggle, weaponToToggle.gameObject.transform.localPosition, new Vector3(0, weaponToToggle.equippedWeapon.aimingYPos, 0), .25f));
    }

    void MoveCameraToDefaultPosition(WeaponShooting weaponToToggle)
    {
        if (cameraFOVLerpCoroutine != null)
            StopCoroutine(cameraFOVLerpCoroutine);

        cameraFOVLerpCoroutine = StartCoroutine(LerpCameraFOV(defaultFOV, timeToLeaveADS));

        if (weaponLocalPositionLerpCoroutine != null)
            StopCoroutine(weaponLocalPositionLerpCoroutine);
 
        weaponLocalPositionLerpCoroutine = StartCoroutine(LerpWeaponLocalPosition(weaponToToggle, weaponToToggle.gameObject.transform.localPosition, new Vector3(0, weaponToToggle.equippedWeapon.defaultYPos, 0), .2f));
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

    IEnumerator LerpWeaponLocalPosition(WeaponShooting weaponToLerp, Vector3 startingPosition, Vector3 finalPosition, float duration)
    {
        float _timeElapsed = 0;

        while (_timeElapsed < duration)
        {
            float t = _timeElapsed / duration;

            weaponToLerp.gameObject.transform.localPosition = Vector3.Lerp(startingPosition, finalPosition, t);
            _timeElapsed += Time.deltaTime;

            yield return null;
        }
        weaponToLerp.gameObject.transform.localPosition = finalPosition;
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
