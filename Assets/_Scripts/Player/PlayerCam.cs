using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    [HideInInspector] public Transform orientation;

    float xRotation;
    float yRotation;

    float defaultFOV;
    float timeToLeaveADS = 0.75f;


    Camera weaponCamera;
    [SerializeField]
    AnimationCurve ADSCurve;

    Coroutine cameraFOVLerp;
    Coroutine weaponLocalPositionLerp;

    private void Awake()
    {
        weaponCamera = GetComponent<Camera>();
        orientation = GameObject.FindGameObjectWithTag("PlayerOrientation").transform;
    }

    private void OnEnable()
    {
        WeaponShooting.onAimDownSights += ToggleAiming;
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
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
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
        if (cameraFOVLerp != null)
            StopCoroutine(cameraFOVLerp);

        cameraFOVLerp = StartCoroutine(LerpCameraFOV(weaponToToggle.equippedWeapon.aimingFOV, weaponToToggle.equippedWeapon.timeToADS));

        if (weaponLocalPositionLerp != null)
            StopCoroutine(weaponLocalPositionLerp);

        weaponLocalPositionLerp = StartCoroutine(LerpWeaponLocalPosition(weaponToToggle, weaponToToggle.gameObject.transform.localPosition, new Vector3(0, weaponToToggle.equippedWeapon.aimingYPos, 0), .25f));
    }

    void MoveCameraToDefaultPosition(WeaponShooting weaponToToggle)
    {
        if (cameraFOVLerp != null)
            StopCoroutine(cameraFOVLerp);

        cameraFOVLerp = StartCoroutine(LerpCameraFOV(defaultFOV, timeToLeaveADS));

        if (weaponLocalPositionLerp != null)
            StopCoroutine(weaponLocalPositionLerp);
 
        weaponLocalPositionLerp = StartCoroutine(LerpWeaponLocalPosition(weaponToToggle, weaponToToggle.gameObject.transform.localPosition, new Vector3(0, weaponToToggle.equippedWeapon.defaultYPos, 0), .2f));
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
