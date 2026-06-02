using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class squid2 : MonoBehaviour
{
    [SerializeField] private DetectionZone detectionZone;

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
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right) walkDirectionVector = Vector2.right;
                else if (value == WalkableDirection.Left) walkDirectionVector = Vector2.left;
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
        get { return animator.GetBool(AnimationStrings.canMove); }
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
        }
    }

    private void OnEnable()
    {
        detectionZone.playerDetected.AddListener(HandlePlayerDetected);
        detectionZone.playerLost.AddListener(HandlePlayerLost);
    }

    private void OnDisable()
    {
        detectionZone.playerDetected.RemoveListener(HandlePlayerDetected);
        detectionZone.playerLost.RemoveListener(HandlePlayerLost);
    }

    void Update()
    {
        // Just sync hasTarget with what's in the attack zone.
        // The animator handles when to attack (via Move -> Attack transition with hasTarget condition).
        // The cooldown is handled by AttackCooldownBeh on the Attack state.
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall && !wasOnWall)
        {
            FlipDirection();
        }

        if (touchingDirections.IsGrounded && cliffDetectionZone != null && cliffDetectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }

        wasOnWall = touchingDirections.IsOnWall;

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
        if (WalkDirection == WalkableDirection.Right) WalkDirection = WalkableDirection.Left;
        else if (WalkDirection == WalkableDirection.Left) WalkDirection = WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (!touchingDirections.IsGrounded) return;
        FlipDirection();
    }

    private void HandlePlayerDetected()
    {
        if (!touchingDirections.IsGrounded) return;

        walkDirectionVector = (playerPos.x > transform.position.x) ? Vector2.right : Vector2.left;
        WalkDirection = (walkDirectionVector == Vector2.right) ? WalkableDirection.Right : WalkableDirection.Left;
    }

    private void HandlePlayerLost()
    {
    }
}