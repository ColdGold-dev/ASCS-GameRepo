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
    public enum AttackType { High, Mid, Low }

    [Header("Attack Cooldown (original cooldown - leave alone)")]
    public float attackCooldownTime = 4f;
    private float attackCooldown = 0f;

    [Header("Random Attack Timer (separate timer for picking next random attack)")]
    [Tooltip("Seconds between random attack picks")]
    public float randomAttackInterval = 3f;
    private float randomAttackTimer = 0f;

    private AttackType lastAttack;

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
        // Tick BOTH timers
        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        if (randomAttackTimer > 0f) randomAttackTimer -= Time.deltaTime;

        HasTarget = attackZone.detectedColliders.Count > 0;

        // Use the random attack timer for the new system
        if (HasTarget && randomAttackTimer <= 0f)
        {
            FireRandomAttack();
            randomAttackTimer = randomAttackInterval; // reset only the random timer
        }
    }

    private void FireRandomAttack()
    {
        int pick = Random.Range(0, 3);

        if (pick == 0)
        {
            lastAttack = AttackType.High;
            animator.SetTrigger("AttackHigh");
            Debug.Log("SQUID | fired AttackHigh");
        }
        else if (pick == 1)
        {
            lastAttack = AttackType.Mid;
            animator.SetTrigger("AttackMid");
            Debug.Log("SQUID | fired AttackMid");
        }
        else
        {
            lastAttack = AttackType.Low;
            animator.SetTrigger("AttackLow");
            Debug.Log("SQUID | fired AttackLow");
        }
    }

    public AttackType GetLastAttackType()
    {
        return lastAttack;
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