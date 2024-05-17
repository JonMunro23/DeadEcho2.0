using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    WeaponData weapon;
    WeaponShooting weaponShooting;

    [Header("Settings")]
    public bool canSway = true;
    public bool sway = true;
    public bool swayRotation = true;
    public bool bobOffset = true;
    public bool bobSway = true;
    [SerializeField] bool isAiming;

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
    public Vector3 HipFireTravelLimit;
    public Vector3 HipFireBobLimit;
    [Space]
    public Vector3 ADSTravelLimit;
    public Vector3 ADSBobLimit;
    [Space]
    public Vector3 currentTravelLimit;
    public Vector3 currentBobLimit;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    [Header("MovementBob")]
    Vector3 movementBob;
    [SerializeField] float movementBobYFrequency;
    [SerializeField] float movementBobXFrequency;
    [SerializeField] float movementBobXAmplitude;

    [Header("Gun Bone")]
    //public Vector3 currentGunBonePos;
    //public Quaternion currentGunBoneRot;
    //[SerializeField] Vector3 newGunBonePos;
    //[SerializeField] Quaternion newGunBoneRot;
    [SerializeField] float gunBoneAimTransitionSmooth;
    [SerializeField] float gunBoneAimTransitionLength;
    [SerializeField] float movementStateSmooth;
    [SerializeField] float movementStateTransitionLength;
    [SerializeField] Vector3 currentMovementStatePos;
    [SerializeField] Quaternion currentMovementStateRot;
    [SerializeField] Vector3 newMovementStatePos;
    [SerializeField] Vector3 newMovementStateRot;
    [SerializeField]
    Transform sprintingState, crouchState;
    PlayerMovement.MovementState currentMovementState;
    Coroutine movementCoroutine, rotationCoroutine;
    bool isLerpingPos, isLerpingRot;

    Vector2 walkInput;
    Vector2 lookInput;

    [Header("Recoil")]
    public Vector3 recoilPosOffset;
    public Vector3 recoilRotOffset;
    [SerializeField] float recoilSmooth;
    [SerializeField]
    float zRecoilAmount, aimingZRecoilAmount, xRotAmount, aimingXRotAmount, yRotAmount, aimingYRotAmount, zRotAmount, aimingZRotAmount;

    private void OnEnable()
    {
        WeaponShooting.onWeaponFired += AddRecoil;
        WeaponShooting.onAimDownSights += ToggleAiming;
        WeaponSwapping.onWeaponSwapped += GetWeaponData;
        PlayerHealth.onDeath += StopSway;
        PlayerMovement.onMovementStateChanged += CheckMovementState;
    }

    private void OnDisable()
    {
        WeaponShooting.onWeaponFired -= AddRecoil;
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponSwapping.onWeaponSwapped -= GetWeaponData;
        PlayerHealth.onDeath -= StopSway;
        PlayerMovement.onMovementStateChanged -= CheckMovementState;
    }

    private void Start()
    {
        //RestoreBob();
    }
    void Update()
    {
        //if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen && canSway)
        //    GetInput();
        //    Sway();
        //    SwayRotation();
        //    BobOffset();
        //    BobRotation();

        //    if (PlayerMovement.instance.isSprinting)
        //        MovementBob();
        //    else
        //        movementBob = Vector3.Lerp(movementBob, Vector3.zero, 4);

        //if (!isAiming)
        //{
        //        LerpToNewMovementState();
        //}

        //LerpRecoil();
    }

    private void FixedUpdate()
    {
        //if(!isAiming)
        //    CompositePositionRotation();

        if(isAiming && !isLerpingPos && !isLerpingRot)
        {
            Debug.Log("Setting");
            transform.localPosition = weapon.gunBoneAimingPos;
            transform.localRotation =  Quaternion .Euler(weapon.gunBoneAimingRot);
        }
    }
    void CheckMovementState(PlayerMovement.MovementState stateToCheck)
    {
        currentMovementState = stateToCheck;
        
        if (isAiming)
            return;

        switch (stateToCheck)
        {
            case PlayerMovement.MovementState.Idle:
                MoveTo(weapon.gunBoneHipPos, weapon.gunBoneHipRot, movementStateTransitionLength);
                //SetNewMovementStatePos(weapon.gunBoneHipPos);
                //SetNewMovementStateRot(weapon.gunBoneHipRot);
                break;
            case PlayerMovement.MovementState.Sprinting:
                MoveTo(sprintingState.localPosition, sprintingState.localRotation.eulerAngles, movementStateTransitionLength);
                //SetNewMovementStatePos(sprintingState.localPosition);
                //SetNewMovementStateRot(sprintingState.localRotation.eulerAngles);
                break;
            case PlayerMovement.MovementState.Crouching:
               MoveTo(crouchState.localPosition, crouchState.localRotation.eulerAngles, movementStateTransitionLength);
                //SetNewMovementStatePos(crouchState.localPosition);
                //SetNewMovementStateRot(crouchState.localRotation.eulerAngles);
                break;
        }

    }

    void SetNewMovementStatePos(Vector3 newPos)
    {
        newMovementStatePos = newPos;
    }

    void SetNewMovementStateRot(Vector3 newRot)
    {
        newMovementStateRot = newRot;
    }

    void LerpToNewMovementState()
    {
        currentMovementStatePos = Vector3.Lerp(currentMovementStatePos, newMovementStatePos + swayPos + bobPosition, movementStateSmooth * Time.deltaTime);
        currentMovementStateRot = Quaternion.Slerp(currentMovementStateRot, Quaternion.Euler(newMovementStateRot), movementStateSmooth * Time.deltaTime);
    }

    void GetWeaponData(GameObject _weapon)
    {
        weaponShooting = _weapon.GetComponent<WeaponShooting>();
        weapon = weaponShooting.weaponData;
        CheckMovementState(currentMovementState);
    }
    private void MovementBob()
    {
        movementBob.y = (Mathf.Sin(Time.time * movementBobYFrequency) / 1000);
        movementBob.x = ((Mathf.Sin(Time.time * movementBobXFrequency) * movementBobXAmplitude) / 1000);
    }

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

    //void SetNewGunBonePos(Vector3 newPos)
    //{
    //    newGunBonePos = newPos;
    //}
    //void LerpGunBonePos()
    //{
    //    currentGunBonePos = Vector3.Lerp(currentGunBonePos, newGunBonePos, gunBoneAimTransitionSmooth * Time.deltaTime);
    //}

    void LerpRecoil()
    {
        recoilPosOffset = Vector3.Lerp(recoilPosOffset, Vector3.zero, recoilSmooth * Time.deltaTime);
        recoilRotOffset = Vector3.Slerp(recoilRotOffset, Vector3.zero, recoilSmooth * Time.deltaTime);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = currentMovementStatePos + recoilPosOffset + movementBob;
        transform.localRotation = Quaternion.Euler(currentMovementStateRot.eulerAngles + recoilRotOffset);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (PlayerMovement.isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * bobExaggeration : 1f) + 0.01f;
        
        if (bobOffset == false) { bobPosition = Vector3.zero; return; }       

        bobPosition.x = (curveCos * currentBobLimit.x * (PlayerMovement.isGrounded ? 1 : 0)) - (walkInput.x * currentTravelLimit.x);
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

    void AddRecoil(bool isAiming)
    {
        //if(isAiming)
        //{
        //    recoilPosOffset += new Vector3(weapon.recoilData.aimingPosRecoil.x, weapon.recoilData.aimingPosRecoil.y, weapon.recoilData.aimingPosRecoil.z);
        //    recoilRotOffset += new Vector3(weapon.recoilData.aimingRotRecoil.x, weapon.recoilData.aimingPosRecoil.y, weapon.recoilData.aimingPosRecoil.z);

        //    MoveTo(weapon.gunBoneAimingPos, weapon.gunBoneAimingRot, gunBoneAimTransitionLength);
        //    return;
        //}

        //recoilPosOffset += new Vector3(weapon.recoilData.posRecoil.x, weapon.recoilData.posRecoil.y, weapon.recoilData.posRecoil.z);
        //recoilRotOffset += new Vector3(weapon.recoilData.rotRecoil.x, weapon.recoilData.posRecoil.y, weapon.recoilData.posRecoil.z);

        //MoveTo(weapon.gunBoneHipPos, weapon.gunBoneHipRot, gunBoneAimTransitionLength);
    }

    public void ToggleAiming(bool _isAiming, WeaponShooting weaponToToggle)
    {
        isAiming = !_isAiming;
        if (_isAiming)
        {
            //RestoreBob();
            CheckMovementState(currentMovementState);
        }
        else if (!_isAiming)
        {
            //ReduceBob();
            MoveTo(weapon.gunBoneAimingPos, weapon.gunBoneAimingRot, gunBoneAimTransitionLength);
            //SetNewMovementStatePos(weapon.gunBoneAimingPos);
            //SetNewMovementStateRot(weapon.gunBoneAimingRot);
        }
    }
    public void ReduceBob()
    {
        currentBobLimit = ADSBobLimit;
        currentTravelLimit = ADSTravelLimit;
    }
    public void RestoreBob()
    {
        currentBobLimit = HipFireBobLimit;
        currentTravelLimit = HipFireTravelLimit;
    }

    void StopSway()
    {
        canSway = false;
    }

    void MoveTo(Vector3 targetPos, Vector3 targetRot, float duration)
    {
        if(movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        if(rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        movementCoroutine = StartCoroutine(LerpPosition(targetPos, duration));
        rotationCoroutine = StartCoroutine(LerpRotation(targetRot, duration));
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        isLerpingPos = true;
        float time = 0;
        Vector3 startPosition = transform.localPosition;

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
        currentMovementStatePos = transform.localPosition;
        isLerpingPos = false;
    }

    IEnumerator LerpRotation(Vector3 targetRotation, float duration)
    {
        isLerpingRot = true;
        float time = 0;
        Quaternion startRotation = transform.localRotation;

        while (time < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.Euler(targetRotation), time / duration);

            time += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotation);
        currentMovementStateRot = transform.localRotation;
        isLerpingRot = false;
    }
}