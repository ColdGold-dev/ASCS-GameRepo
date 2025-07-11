using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement2D : MonoBehaviour
{
    #region Debug
    [Header("Debug")]
    [SerializeField] private GameObject debugGroundCheck;
    [SerializeField] private GameObject debugCrouchCheck;
    #endregion

    #region Input Actions
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference attackAction;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 20f)] private float moveSpeed = 8f;
    [SerializeField, Range(1f, 3f)] private float sprintMultiplier = 1.5f;
    [SerializeField, Range(0.1f, 1f)] private float crouchSpeedMultiplier = 0.5f;
    #endregion

    #region Jump Settings
    [Header("Jump Settings")]
    [SerializeField, Range(1f, 30f)] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField, Tooltip("How far below the feet to look for ground")]
    private float groundCheckDistance = 0.05f;
    #endregion

    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Vector2 movementInput;
    private Animator myAnimator;
    // State variables
    private bool isCrouching;
    private bool isSprinting;
    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking;
    private bool isIdle => !isCrouching && !isSprinting && !isAttacking && isGrounded && movementInput == Vector2.zero;

    private float currentSpeedMultiplier = 1f;
    private Component attackHandler;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();

        // Automatically find the Attacks component in children
        attackHandler = gameObject.GetComponentInChildren<AttackSystem>();
        if (attackHandler == null)
        {
            Debug.LogError("No Attacks component found in children!");
        }

#if UNITY_EDITOR
        if (debugGroundCheck) debugGroundCheck.SetActive(false);
        if (debugCrouchCheck) debugCrouchCheck.SetActive(false);
#endif
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
        crouchAction?.action.Enable();
        sprintAction?.action.Enable();
        attackAction?.action.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        jumpAction?.action.Disable();
        crouchAction?.action.Disable();
        sprintAction?.action.Disable();
        attackAction?.action.Disable();
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovementInput();
        HandleCrouch();
        HandleSprint();
        HandleJump();
        HandleAttack();
        HandleAnimator();

    }

    /// <summary>
    /// Checks if the player is touching the ground using a raycast
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position +
                         Vector2.down * (playerCollider.bounds.extents.y + 0.5f);

        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);
        isGrounded = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);

#if UNITY_EDITOR
        if (debugGroundCheck) debugGroundCheck.SetActive(isGrounded);
#endif
    }

    /// <summary>
    /// Handles horizontal movement based on player input
    /// </summary>
    private void HandleMovementInput()
    {
        movementInput = moveAction.action.ReadValue<Vector2>();
         float horizontalVelocity = movementInput.x * moveSpeed * currentSpeedMultiplier;
         playerRigidbody.linearVelocity  = new Vector2(horizontalVelocity, playerRigidbody.linearVelocity .y);
        //flip the player sprite based on movement direction
        if (movementInput.x > 0)
        {
            print("Moving right");
            transform.localScale = new Vector3(4, 4, 4); // Facing right
        }
        else if (movementInput.x < 0)
        {
            print("Moving left");
            transform.localScale = new Vector3(-4, 4, 4); // Facing left

        }
        movementInput = moveAction.action.ReadValue<Vector2>();
         horizontalVelocity = movementInput.x * moveSpeed * currentSpeedMultiplier;
        playerRigidbody.linearVelocity = new Vector2(horizontalVelocity, playerRigidbody.linearVelocity.y);

        //if falling
        if (playerRigidbody.linearVelocity.y < 0)
        {
            isJumping = false; // Reset jumping state when falling
        }

    }

    /// <summary>
    /// Handles crouch state and applies speed modifier
    /// </summary>
    private void HandleCrouch()
    {
        isCrouching = crouchAction.action.IsPressed();
        currentSpeedMultiplier = isCrouching ? crouchSpeedMultiplier : 1f;

#if UNITY_EDITOR
        if (debugCrouchCheck) debugCrouchCheck.SetActive(isCrouching);
#endif
    }

    /// <summary>
    /// Handles sprint state and applies speed modifier if not crouching
    /// </summary>
    private void HandleSprint()
    {
        isSprinting = sprintAction.action.IsPressed() && !isCrouching;

        if (isSprinting && isGrounded)
        {
            currentSpeedMultiplier = sprintMultiplier;
            isSprinting = true;
        }
        else if (!isCrouching)
        {
            currentSpeedMultiplier = 1f;
            isSprinting = false;
        }
    }

    /// <summary>
    /// Handles jumping when the player presses the jump button and is grounded
    /// </summary>
    private void HandleJump()
    {
        //isJumping = false;
        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            isJumping = true;
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, jumpForce);
            
            //Run jump anaimation if available
        }
    }

    /// <summary>
    /// Handles attack action when the player presses the attack button
    /// </summary>
    private void HandleAttack()
    {
        if (attackHandler == null)
        {
            Debug.LogError("WARNING: attackHandler is not assigned in the inspector.");
            return;
        }

        if (attackAction.action.WasPressedThisFrame())
        {
           //attackHandler.ActivateAttack(attackTime);
            isAttacking = true;

        }
        
    }

    public void HandleAnimator()
    {
        myAnimator.SetBool("isCrouching", isCrouching);
        myAnimator.SetBool("isSprinting", isSprinting);
        myAnimator.SetBool("isGrounded", isGrounded);
        myAnimator.SetBool("isJumping", isJumping);
        myAnimator.SetBool("isAttacking", isAttacking);
        myAnimator.SetBool("isIdle", isIdle);

        isAttacking = false; // Reset attacking state after handling animation
        
    }

    private void animationHandler()
    {
        if (myAnimator == null)
        {
            Debug.LogError("Animator not found and or assigned");
            return;
        }

        myAnimator.SetBool("isCrouching", isCrouching);
        myAnimator.SetBool("isSprinting", isSprinting);
        myAnimator.SetBool("isJumping", isJumping);
        myAnimator.SetBool("isAttacking", isAttacking);
        myAnimator.SetBool("isIdle", isIdle);
    }
}