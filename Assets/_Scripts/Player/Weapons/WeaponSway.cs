using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public PlayerMovement playerMovement;

    WeaponData weapon;
    WeaponShooting weaponShooting;

    [Header("Settings")]
    public bool canSway = true;
    public bool sway = true;
    public bool swayRotation = true;
    public bool bobOffset = true;
    public bool bobSway = true;

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }
    [Space]
    public float HipFireTravelLimit;
    public float HipFireBobLimit;
    [Space]
    public float ADSTravelLimit;
    public float ADSBobLimit;
    [Space]
    public Vector3 currentTravelLimit;
    public Vector3 currentBobLimit;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    Vector3 defaultPos;
    Vector3 aimingPos;

    Vector3 startPos;

    private void OnEnable()
    {
        WeaponShooting.onAimDownSights += ToggleAiming;
        WeaponSwapping.onWeaponSwapped += GetWeaponData;
        PlayerHealth.onDeath += StopSway;
    }

    private void OnDisable()
    {
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponSwapping.onWeaponSwapped -= GetWeaponData;
        PlayerHealth.onDeath -= StopSway;
    }
    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        startPos = transform.localPosition;

        RestoreBob();
    }

    void GetWeaponData(GameObject _weapon)
    {
        weaponShooting = _weapon.GetComponent<WeaponShooting>();
        weapon = weaponShooting.weaponData;
        aimingPos = weaponShooting.weaponData.gunBoneAimingPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen && canSway)
            GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        //if(!weaponShooting.isReloading)
            CompositePositionRotation();


    }


    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        walkInput.x = Input.GetAxis("Horizontal");
        walkInput.y = Input.GetAxis("Vertical");
        walkInput = walkInput.normalized;

        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }


    void Sway()
    {
        if(sway == false) { swayPos = Vector3.zero; return; }

        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        if (swayRotation == false) { swayEulerRot = Vector3.zero; return; }

        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        Vector3 pos = weaponShooting.isAiming ? aimingPos : startPos;

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition + pos, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (playerMovement.isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * bobExaggeration : 1f) + 0.01f;
        
        if (bobOffset == false) { bobPosition = Vector3.zero; return; }       

        bobPosition.x = (curveCos * currentBobLimit.x * (playerMovement.isGrounded ? 1 : 0)) - (walkInput.x * currentTravelLimit.x);
        bobPosition.y = (curveSin * currentBobLimit.y) - (Input.GetAxis("Vertical") * currentTravelLimit.y);
        bobPosition.z = -(walkInput.y * currentTravelLimit.z);
    }

    void BobRotation()
    {
        if (bobSway == false) { bobEulerRotation = Vector3.zero; return; }

        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }

    public void ToggleAiming(bool _isAiming, WeaponShooting weaponToToggle)
    {
        if (_isAiming)
        {
            RestoreBob();
        }
        else if (!_isAiming)
        {
            ReduceBob();
        }
    }
    public void ReduceBob()
    {
        currentBobLimit = new Vector3(ADSBobLimit, ADSBobLimit, ADSBobLimit); 
        currentTravelLimit = new Vector3(ADSTravelLimit, ADSTravelLimit, ADSTravelLimit);
    }
    public void RestoreBob()
    {
        currentBobLimit = new Vector3(HipFireBobLimit, HipFireBobLimit, HipFireBobLimit);
        currentTravelLimit = new Vector3(HipFireTravelLimit, HipFireTravelLimit, HipFireTravelLimit);
    }

    void StopSway()
    {
        canSway = false;
    }
}