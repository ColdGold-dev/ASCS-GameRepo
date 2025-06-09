using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement2D : MonoBehaviour
{
    //debug will be removed in final production version
    [Header("Debug")]
    [SerializeField] GameObject debugGroundCheck;
    [SerializeField] GameObject CrouchCheck;



    [Header("InputAction")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;
    [SerializeField] InputActionReference crouchAction;
    [SerializeField] InputActionReference sprintAction;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float sprintMultiplier = 1.5f;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpBufferTime = 0.2f; // Time window for jump buffering
    [SerializeField] float coyoteTime = 0.1f; // Time window for coyote time

    [SerializeField] LayerMask groundMask;
    [Tooltip("How far below the feet to look for ground")]
    [SerializeField] float groundCheckDistance = 0.05f;

    Rigidbody2D rb;
    Collider2D col;
    Vector2 input;
    //bool jumpHeld;
    bool crouchHeld;
    bool sprintHeld;
    bool isGrounded;
    float lastGroundedTime = -100f;
    float lastJumpPressTime = -100f;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        crouchAction.action.Enable();
        sprintAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        crouchAction.action.Disable();
        sprintAction.action.Disable();
    }

    void Update()
    {
        IsGrounded();
        lefRightMovement();
        crouchManager();
        sprintManager();
        jumpControl();
    }
    //To Do add more raycasts
    void IsGrounded()
    {
        // Cast straight down from just inside the bottom of the collider
        Vector2 origin = (Vector2)transform.position +
                         Vector2.down * (col.bounds.extents.y - 0.01f);

        // Visualize the ray in scene view
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);

        isGrounded = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
    }

    void jumpControl()
    {
        if (jumpAction.action.WasPressedThisFrame())
        {
            lastJumpPressTime = Time.time;
        }
        //bool grounded = IsGrounded();
        if (isGrounded)
        {
            lastGroundedTime = Time.time;

        }

        bool canJump = (Time.time - lastGroundedTime < coyoteTime) &&
                      (Time.time - lastJumpPressTime < jumpBufferTime);

        //Jump If CanJump is true
        if (canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpPressTime = 0;
            lastGroundedTime = 0;

        }
        //Debug
        debugGroundCheck.SetActive(isGrounded);
    }


    void lefRightMovement()
    {
        input = moveAction.action.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(input.x * moveSpeed * speedManager(), rb.linearVelocity.y);
    }

    void crouchManager()
    {
        crouchHeld = crouchAction.action.IsPressed();

        //debug
        CrouchCheck.SetActive(crouchHeld);
    }
    void sprintManager()
    {
        sprintHeld = sprintAction.action.IsPressed();

    }

    float speedManager()
    {
        if (sprintHeld && isGrounded)
        {
            return sprintMultiplier;
        }
        //Normal Speed
        return 1;
    }


}