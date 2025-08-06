using UnityEngine;

//cha
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class squid2 : MonoBehaviour
{
    [SerializeField] private DetectionZone detectionZone;
   
    // Tracks previous wall state for one-time flips
private bool wasOnWall = false;

    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection { Right, Left }

    public float attackCooldownTime = 2f; // You can edit this in the Inspector

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    private Vector3 playerPos => GameMasterScript.Instance.Player.GetPlayerPosition();


    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value;
        }
    }






    public bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    //chatgbtaddedforattackcoodedown

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

 private void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    touchingDirections = GetComponent<TouchingDirections>();
    animator = GetComponent<Animator>();
    damageable = GetComponent<Damageable>();
    detectionZone = GetComponentInChildren<DetectionZone>();

    GroundDetection groundDetection = GetComponentInChildren<GroundDetection>();
    if (groundDetection != null)
    {
        groundDetection.onCliffDetected.AddListener(OnCliffDetected);
        Debug.Log($"{name}: Subscribed to GroundDetection.onCliffDetected");
    }
    else
    {
        
    }
}

    private void OnEnable()
    {
        // Subscribe to events
        detectionZone.playerDetected.AddListener(HandlePlayerDetected);
        detectionZone.playerLost.AddListener(HandlePlayerLost);
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        detectionZone.playerDetected.RemoveListener(HandlePlayerDetected);
        detectionZone.playerLost.RemoveListener(HandlePlayerLost);
    }

    // Update is called once per frame


    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (HasTarget && AttackCooldown <= 0f)
        {
            animator.SetTrigger("Attack");
            AttackCooldown = attackCooldownTime; // use the public cooldown time
            animator.SetBool(AnimationStrings.canMove, false); // optional: freeze during attack
        }

        if (AttackCooldown > 0f)
        {
            AttackCooldown -= Time.deltaTime;
        }


    }

private void FixedUpdate()
{
    // Flip once when hitting a wall (false → true)
    if (touchingDirections.IsGrounded && touchingDirections.IsOnWall && !wasOnWall)
    {
        FlipDirection();
    }

    // Flip if no ground detected in cliff detection zone
    if (cliffDetectionZone != null && cliffDetectionZone.detectedColliders.Count == 0)
    {
        FlipDirection();
    }

    // Update previous wall state
    wasOnWall = touchingDirections.IsOnWall;

    // Movement handling
    if (!damageable.LockVelocity)
    {
        if (CanMove && touchingDirections.IsGrounded)
        {
            rb.linearVelocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
                rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
        }
    }
}

    public void FlipDirection()
    {
            Debug.Log($"{name}: FLIPPING via GroundDetection event");
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            //  Debug.LogError("Current walkable direction is not set to legal values of right or left");
        }
    }

    // called when hit by a attack
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

public void OnCliffDetected()
{
    Debug.Log($"{name}: OnCliffDetected() called");
    FlipDirection();
}




    
    private void HandlePlayerDetected()
    {
        //    print("X" + playerPos.x + "Y" + playerPos.y);
        walkDirectionVector = (playerPos.x > transform.position.x) ? Vector2.right : Vector2.left;
        //flip direction based on player position
        WalkDirection = (walkDirectionVector == Vector2.right) ? WalkableDirection.Right : WalkableDirection.Left;

    }

  

    private void HandlePlayerLost()
    {
        //logic to handle when player is lost
    }

}
