using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement")]
    public float baseMoveSpeed;
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [SerializeField] bool readyToJump, hasJumped;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Animator animator;



    [Header("MovementSFX")]
    [SerializeField] float walkingSFXCooldown;
    [SerializeField] AudioSource movementAudioSource, landingAudioSource;
    [SerializeField] AudioClip[] walkingSFX, landingSFX;
    bool canPlaySFX;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        moveSpeed = baseMoveSpeed;
        rb.freezeRotation = true;
        readyToJump = true;

        canPlaySFX = true;
    }

    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //begin sprinting
        if(Input.GetKey(sprintKey) && grounded)
        {

        }

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
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
        if(grounded)
        {
            if(hasJumped)
            {
                hasJumped = false;
                landingAudioSource.PlayOneShot(PickSFXClip(landingSFX));
                Debug.Log("Played landing sfx");
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            if(horizontalInput != 0 || verticalInput != 0)
            {
                animator.SetFloat("speed", 0.5f, .2f, Time.deltaTime);
                if(canPlaySFX)
                {
                    canPlaySFX = false;
                    movementAudioSource.PlayOneShot(PickSFXClip(walkingSFX));
                    StartCoroutine(WalkingSFXCooldown());
                }
            }
            else
            {
                animator.SetFloat("speed", 0.0f);
            }
        }

        // in air
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            animator.SetFloat("speed", 0.0f, .2f, Time.deltaTime);
        }
    }

    AudioClip PickSFXClip(AudioClip[] arrayToPickFrom)
    {
        int rand = Random.Range(0, arrayToPickFrom.Length);
        return walkingSFX[rand];
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;
        hasJumped = true;
    }

    public void BeginAiming()
    {
        moveSpeed = baseMoveSpeed / 4;
    }

    public void StopAiming()
    {
        moveSpeed = baseMoveSpeed;
    }


    IEnumerator WalkingSFXCooldown()
    {
        yield return new WaitForSeconds(walkingSFXCooldown);
        canPlaySFX = true;
    }
}