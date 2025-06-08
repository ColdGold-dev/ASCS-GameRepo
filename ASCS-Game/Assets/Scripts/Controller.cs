using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement2D : MonoBehaviour
{
    // ─────────────── INPUT ───────────────
    [Header("Input Actions (drag from .inputactions asset)")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;

    // ───────────── MOVEMENT ──────────────
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;

    // ───────────── JUMPING ───────────────
    [Header("Jumping")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float jumpCutMultiplier = 0.5f;
    [SerializeField] float jumpBufferTime = 0.2f; // Time window for jump buffering
    [SerializeField] float coyoteTime = 0.1f; // Time window for coyote time

    // ───────────── GROUNDING ─────────────
    [SerializeField] LayerMask groundMask;
    [Tooltip("How far below the feet to look for ground")]
    [SerializeField] float groundCheckDistance = 0.05f;

    // ─────────── PRIVATE STATE ───────────
    Rigidbody2D rb;
    Collider2D  col;
    Vector2 input;
    bool jumpPressed;
    bool jumpHeld;
    bool facingRight = true;
    float lastGroundedTime;
    float lastJumpPressTime;

    /* ───────────── LIFECYCLE ───────────── */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }
    
    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        // Read input
        input = moveAction.action.ReadValue<Vector2>();
        
        // Handle jump input
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpPressed = true;
            lastJumpPressTime = Time.time;
        }
        
        jumpHeld = jumpAction.action.IsPressed();
    }

    void FixedUpdate()
    {
        // Ground check
        bool grounded = IsGrounded();
        if (grounded)
        {
            lastGroundedTime = Time.time;
        }

        // Horizontal movement
        rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);

        // Jump handling with buffer and coyote time
        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) && 
                      (Time.time - lastJumpPressTime <= jumpBufferTime);
        
        if (canJump)
        {
            // Jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            // Reset jump state
            jumpPressed = false;
            lastJumpPressTime = 0;
        }

        // Optional: Jump cut (short hop)
        if (jumpHeld == false && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    bool IsGrounded()
    {
        // Cast straight down from just inside the bottom of the collider
        Vector2 origin = (Vector2)transform.position +
                         Vector2.down * (col.bounds.extents.y - 0.01f);

        // Visualize the ray in scene view
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);
        
        return Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);
    }
}