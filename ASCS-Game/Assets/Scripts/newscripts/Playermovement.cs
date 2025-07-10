using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Playermovement : MonoBehaviour
{
    private TouchingDirections touchingDirections;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    public float jumpImpulse = 10f;

    [SerializeField]
    private bool _IsMoving = false;

    [SerializeField]
    private bool _IsRunning = false;

    public float CurrentMoveSpeed
    {
        get
        {
            if (canMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    return IsRunning ? runSpeed : walkSpeed;
                }
                return 0;
            }

            return 0;
        }
    }



    public bool IsMoving
    {
        get { return _IsMoving; }
        private set
        {
            _IsMoving = value;
            animator.SetBool(AnimationStrings.IsMoving, value);
        }
    }

    public bool IsRunning
    {
        get { return _IsRunning; }
        set
        {
            _IsRunning = value;
            animator.SetBool(AnimationStrings.IsRunning, value);
        }
    }

    public bool _isFacingRight = true;




    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            _isFacingRight = value;
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }


    private void Awake()
    {
       
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirecetion(moveInput);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    private void SetFacingDirecetion(Vector2 moveinput)
    {
        if (moveinput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveinput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.Attack);
        }
    }

}