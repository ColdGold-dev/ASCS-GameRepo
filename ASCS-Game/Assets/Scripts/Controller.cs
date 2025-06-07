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

    /* ───────────── LIFECYCLE ───────────── */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Debug.Log($"Layers in groundMask: {groundMask.value}");
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
        input       = moveAction.action.ReadValue<Vector2>();
        jumpPressed = jumpAction.action.WasPressedThisFrame();
        jumpHeld    = jumpAction.action.IsPressed();

    
      
    }

    void FixedUpdate()
    {
        // Horizontal move
        rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (jumpPressed || IsGrounded() && jumpAction.action.IsPressed())
        {
            
        }
        if (jumpPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        jumpPressed = false;   // reset single-frame flag

         
    }

    bool IsGrounded()
    {
        // Cast straight down from just inside the bottom of the collider
        Vector2 origin = (Vector2)transform.position +
                         Vector2.down * (col.bounds.extents.y - 0.01f);

        return Physics2D.Raycast(origin, Vector2.down,
                                 groundCheckDistance, groundMask);
    }

}
