using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement")]
    public bool canMove;
    public float baseMoveSpeed;
    float currentMoveSpeed;
    public float groundDrag;
    public LayerMask groundLayer;
    public Transform orientation;
    //public Animator animator;
    [SerializeField] Transform cameraPos;
    [HideInInspector] public float walkingMovementSpeed;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public Vector3 currentVelocity;
    [SerializeField] Vector3 originalCamPos;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Jumping")]
    [SerializeField] bool readyToJump;
    public float playerHeight;
    public bool isGrounded;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] bool hasJumped;

    [Header("Aiming")]
    public bool isAiming;
    [HideInInspector] public float aimingMovementSpeed;

    [Header("Sprinting")]
    public bool isSprinting;
    public float sprintSpeedMultiplier;
    [HideInInspector] public float sprintingMovementSpeed;
    Coroutine sprintingBreathingSFXCoroutine;

    [Header("Crouching")]
    public bool isCrouching;
    [SerializeField] Vector3 crouchingCamPos;
    [HideInInspector] float crouchingMovementSpeed;
    [SerializeField] float crouchingSpeed;

    [Header("Sliding")]
    public bool isSliding;
    [SerializeField] Vector3 slidingCamPos;
    [SerializeField] int slideForce;
    [SerializeField] float maxSlideLength, slideLength;

    [Header("MovementSFX")]
    [SerializeField] bool canPlayMovementSFX;
    [SerializeField] float walkingSFXCooldown, aimingWalkingSFXCooldown, sprintingSFXCooldown, crouchingSFXCooldown;
    [SerializeField] AudioSource movementAudioSource, jumpingAudioSource, sprintingAudioSource;
    [SerializeField] AudioClip[] walkingSFX, landingSFX;
    [SerializeField] AudioClip jumpingGruntingSFX;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        WeaponShooting.onAimDownSights += ToggleAiming;
        WeaponShooting.onWeaponFired += InterruptSprint;
        WeaponSwapping.onWeaponSwapped += SwapToNewWeaponAnimator;
        PlayerHealth.onDeath += StopMovement;
    }

    void Start()
    {
        currentMoveSpeed = baseMoveSpeed;
        rb.freezeRotation = true;
        readyToJump = true;
        aimingMovementSpeed = baseMoveSpeed / 2;
        sprintingMovementSpeed = baseMoveSpeed * sprintSpeedMultiplier;
        crouchingMovementSpeed = baseMoveSpeed / 3;
        canPlayMovementSFX = true;
    }

    private void OnDisable()
    {
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponShooting.onWeaponFired -= InterruptSprint;
        WeaponSwapping.onWeaponSwapped -= SwapToNewWeaponAnimator;
        PlayerHealth.onDeath -= StopMovement;
    }

    void Update()
    {
        if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen && canMove)
            PlayerInput();

        SpeedControl();
        HandleCrouching();
        //HandleSliding();

        // ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);
        // handle drag
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    void FixedUpdate()
    {
        if(canMove)
            MovePlayer();
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput == -1 && isSprinting)
        {
            StopSprinting(false);
            return;
        }

        //sprinting
        if (Input.GetKeyDown(sprintKey) && isGrounded && !isAiming)
        {
            BeginSprinting();
        }
        if(Input.GetKeyUp(sprintKey) && isSprinting)
        {
            StopSprinting(false);
        }

        //Crouching
        if(Input.GetKey(crouchKey) && !isSprinting && isGrounded)
        {
            Crouch();
        }
        if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            StopCrouching();
        }

        //Sliding
        //if (Input.GetKey(crouchKey) && isSprinting)
        //{
        //    BeginSliding();
        //}

        //Jumping
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded && !isCrouching)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(isGrounded)
        {
            if(hasJumped)
            {
                hasJumped = false;
                jumpingAudioSource.PlayOneShot(PickSFXClip(landingSFX));
            }

            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f, ForceMode.Force);
            if(horizontalInput != 0 || verticalInput != 0)
            {
                //if(isSprinting)
                //    animator.SetFloat("speed", 1, .2f, Time.deltaTime);
                //else if(!isCrouching)
                //    animator.SetFloat("speed", 0.66f, .2f, Time.deltaTime);

                if(canPlayMovementSFX)
                {
                    canPlayMovementSFX = false;
                    movementAudioSource.PlayOneShot(PickSFXClip(walkingSFX));
                    StartCoroutine(FootstepSFXCooldown());
                }
            }
            else
            {
                //animator.SetFloat("speed", 0.0f, .2f, Time.deltaTime);
            }
        }

        // in air
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10f * airMultiplier, ForceMode.Force);
            //animator.SetFloat("speed", 0.0f, .2f, Time.deltaTime);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //CrosshairManager.instance.UpdateCrosshairSize(flatVel.magnitude);
        currentVelocity = flatVel;

        // limit velocity if needed
        if(flatVel.magnitude > currentMoveSpeed && !isSprinting)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        else if (flatVel.magnitude > sprintingMovementSpeed && isSprinting)
        {
            Vector3 limitedVel = flatVel.normalized * sprintingMovementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    #region Crouching
    void Crouch()
    {
        if(!isCrouching)
        {
            isCrouching = true;
            currentMoveSpeed = crouchingMovementSpeed;
            //animator.SetBool("isCrouching", isCrouching);
            movementAudioSource.volume = .5f;
        }
    }

    void StopCrouching()
    {
        if(isCrouching)
        {
            isCrouching = false;
            //animator.SetBool("isCrouching", isCrouching);
            if (GetCurrentWeaponShootingScript().isAiming)
            {
                currentMoveSpeed = aimingMovementSpeed;
            }
            else
                currentMoveSpeed = baseMoveSpeed;

            movementAudioSource.volume = 1;
        }

    }

    void HandleCrouching()
    {
        if (isCrouching == true)
        {
            cameraPos.transform.localPosition = Vector3.MoveTowards(cameraPos.transform.localPosition, crouchingCamPos, crouchingSpeed * Time.deltaTime);
            if(horizontalInput != 0 || verticalInput != 0)
            {
                //animator.SetFloat("speed", 0.33f, .2f, Time.deltaTime);
            }
        }
        else if (isCrouching == false && cameraPos.transform.localPosition.y < originalCamPos.y)
        {
            cameraPos.transform.localPosition = Vector3.MoveTowards(cameraPos.transform.localPosition, originalCamPos, crouchingSpeed * Time.deltaTime);
            //animator.SetFloat("speed", 0, .2f, Time.deltaTime);
        }
    }
    #endregion

    #region Sprinting
    void BeginSprinting()
    {
        if(verticalInput == -1)
        {
            InterruptSprint(false);
            return;
        }

        if(GetCurrentWeaponShootingScript().isAiming)
        {
            GetCurrentWeaponShootingScript().StopADS();
        }

        if(!isSprinting)
        { 
            if(sprintingBreathingSFXCoroutine == null)
                sprintingBreathingSFXCoroutine = StartCoroutine(StartSprintingBreathingSFX());
        }
        isSprinting = true;
        currentMoveSpeed = sprintingMovementSpeed;
    }

    void InterruptSprint(bool interruptedByAiming)
    {
        StopSprinting(interruptedByAiming);
    }

    void StopSprinting(bool stoppedByAiming)
    {
        isSprinting = false;

        if (stoppedByAiming)
            currentMoveSpeed = aimingMovementSpeed;
        else
            currentMoveSpeed = baseMoveSpeed;

        StartCoroutine(_Helpers.FadeOutAudio(sprintingAudioSource, 1.5f));
        if(sprintingBreathingSFXCoroutine != null)
        {
            StopCoroutine(sprintingBreathingSFXCoroutine);
            sprintingBreathingSFXCoroutine = null;
        }
    }
    #endregion

    #region Sliding
    //void BeginSliding()
    //{
    //    if(!isSliding)
    //    {
    //        isSliding = true;
    //        slideLength = maxSlideLength;
    //    }
    //}

    //void HandleSliding()
    //{
    //    if(isSliding)
    //    {
    //        Debug.Log("isSliding = true");
    //        cameraPos.transform.localPosition = Vector3.MoveTowards(cameraPos.transform.localPosition, slidingCamPos, crouchingSpeed * Time.deltaTime);
    //        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

    //        // sliding normal
    //        //if (rb.velocity.y > -0.1f)
    //        //{
    //            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

    //            slideLength -= Time.deltaTime;
    //        //}

    //        if (slideLength <= 0)
    //            StopSliding();
    //    }
    //    else if (!isSliding && cameraPos.transform.localPosition.y < originalCamPos.y)
    //    {
    //        Debug.Log("isSliding = false");
    //        cameraPos.transform.localPosition = Vector3.MoveTowards(cameraPos.transform.localPosition, originalCamPos, crouchingSpeed * Time.deltaTime);
    //    }
    //}

    //void StopSliding()
    //{
    //    if(isSliding)
    //    {
    //        isSliding = false;
    //    }
    //}
    #endregion

    #region Jumping
    void Jump()
    {
        if (GetCurrentWeaponShootingScript().isAiming)
            GetCurrentWeaponShootingScript().StopADS();

        //StopSliding();

        jumpingAudioSource.PlayOneShot(jumpingGruntingSFX);
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;
        hasJumped = true;
    }
    #endregion

    #region Aiming
    

    public void ToggleAiming(bool _isAiming, WeaponShooting weaponAiming)
    {
        if (_isAiming)
        {
            StopAiming();
        }
        else if (!_isAiming)
        {
            BeginAiming();
        }
    }
    public void BeginAiming()
    {
        isAiming = true;
        if (isCrouching)
            currentMoveSpeed = crouchingMovementSpeed;
        else
            currentMoveSpeed = aimingMovementSpeed;

        if (isSprinting)
            StopSprinting(true);
    }

    public void StopAiming()
    {
        isAiming = false;
        if (isCrouching)
        {
            currentMoveSpeed = crouchingMovementSpeed;
        }
        else
            currentMoveSpeed = baseMoveSpeed;
    }
    #endregion

    AudioClip PickSFXClip(AudioClip[] arrayToPickFrom)
    {
        int rand = Random.Range(0, arrayToPickFrom.Length);
        return walkingSFX[rand];
    }
    WeaponShooting GetCurrentWeaponShootingScript()
    {
        //CAN PUT IN HELPERS IF NEEDED
        WeaponShooting weaponShooting = WeaponSwapping.instance.currentlyEquippedWeaponObj.GetComponent<WeaponShooting>();
        return weaponShooting;
    }

    IEnumerator FootstepSFXCooldown()
    {
        if (GetCurrentWeaponShootingScript().isAiming)
            yield return new WaitForSeconds(aimingWalkingSFXCooldown);
        else if(isSprinting == true)
            yield return new WaitForSeconds(sprintingSFXCooldown);
        else if (isCrouching == true)
            yield return new WaitForSeconds(crouchingSFXCooldown);
        else
            yield return new WaitForSeconds(walkingSFXCooldown);

        canPlayMovementSFX = true;
    }

    IEnumerator StartSprintingBreathingSFX()
    {
        yield return new WaitForSeconds(2);
        sprintingAudioSource.Play();
    }

    void SwapToNewWeaponAnimator(GameObject newWeaponObj)
    {
        //animator = newWeaponObj.GetComponent<Animator>();
    }

    void StopMovement()
    {
        canMove = false;
        //animator.SetFloat("speed", 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity =  Vector3.zero;
    }
}