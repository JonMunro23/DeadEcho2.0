using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement")]
    public bool canMove;
    public float baseMoveSpeed;
    public float moveSpeed;
    public float groundDrag;
    public LayerMask groundLayer;
    public Transform orientation;
    public Animator animator;
    [SerializeField] Transform cameraPos;
    [HideInInspector] public float walkingMovementSpeed;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
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
    [HideInInspector] public float aimingMovementSpeed;

    [Header("Sprinting")]
    public bool isSprinting;
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
        WeaponSwapping.onWeaponSwapped += SwapToNewWeaponAnimator;
        PlayerHealth.onDeath += StopMovement;
    }

    void Start()
    {
        
        moveSpeed = baseMoveSpeed;
        rb.freezeRotation = true;
        readyToJump = true;
        aimingMovementSpeed = baseMoveSpeed / 2;
        sprintingMovementSpeed = baseMoveSpeed * 1.66f;
        crouchingMovementSpeed = baseMoveSpeed / 3;
        canPlayMovementSFX = true;
    }

    private void OnDisable()
    {
        WeaponShooting.onAimDownSights -= ToggleAiming;
        WeaponSwapping.onWeaponSwapped -= SwapToNewWeaponAnimator;
        PlayerHealth.onDeath -= StopMovement;
    }

    void Update()
    {
        if(canMove)
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

        //sprinting
        if(Input.GetKey(sprintKey) && isGrounded)
        {
            BeginSprinting();
        }
        if(Input.GetKeyUp(sprintKey))
        {
            StopSprinting();
        }

        //Crouching
        if(Input.GetKey(crouchKey) && !isSprinting && isGrounded)
        {
            Crouch();
        }
        if (Input.GetKeyUp(crouchKey))
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

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            if(horizontalInput != 0 || verticalInput != 0)
            {
                if(isSprinting)
                    animator.SetFloat("speed", 1, .2f, Time.deltaTime);
                else if(!isCrouching)
                    animator.SetFloat("speed", 0.66f, .2f, Time.deltaTime);

                if(canPlayMovementSFX)
                {
                    canPlayMovementSFX = false;
                    movementAudioSource.PlayOneShot(PickSFXClip(walkingSFX));
                    StartCoroutine(FootstepSFXCooldown());
                }
            }
            else
            {
                animator.SetFloat("speed", 0.0f, .2f, Time.deltaTime);
            }
        }

        // in air
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            animator.SetFloat("speed", 0.0f, .2f, Time.deltaTime);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed && !isSprinting)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
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
            moveSpeed = crouchingMovementSpeed;
            animator.SetBool("isCrouching", isCrouching);
            movementAudioSource.volume = .5f;
        }
    }

    void StopCrouching()
    {
        if(isCrouching)
        {
            isCrouching = false;
            animator.SetBool("isCrouching", isCrouching);
            if (GetCurrentWeaponShootingScript().isAiming)
            {
                moveSpeed = aimingMovementSpeed;
            }
            else
                moveSpeed = baseMoveSpeed;

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
                animator.SetFloat("speed", 0.33f, .2f, Time.deltaTime);
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
        if(!isSprinting)
        { 
            if(sprintingBreathingSFXCoroutine == null)
                sprintingBreathingSFXCoroutine = StartCoroutine(StartSprintingBreathingSFX());
        }
        isSprinting = true;
        moveSpeed = sprintingMovementSpeed;
        if(GetCurrentWeaponShootingScript().isAiming)
        {
            GetCurrentWeaponShootingScript().StopADS();
        }
    }

    void StopSprinting()
    {
        isSprinting = false;
        moveSpeed = baseMoveSpeed;
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
    

    public void ToggleAiming(bool _isAiming, WeaponShooting weaponToToggle)
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
        if (isCrouching)
            moveSpeed = crouchingMovementSpeed;
        else
            moveSpeed = aimingMovementSpeed;

        if (isSprinting)
            StopSprinting();
    }
    public void StopAiming()
    {
        if (isCrouching)
        {
            moveSpeed = crouchingMovementSpeed;
        }
        else
            moveSpeed = baseMoveSpeed;
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
        animator = newWeaponObj.GetComponent<Animator>();
    }

    void StopMovement()
    {
        canMove = false;
        animator.SetFloat("speed", 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity =  Vector3.zero;
    }
}