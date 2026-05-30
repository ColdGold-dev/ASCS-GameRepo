using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;

    [HideInInspector]
    public Vector2 launchDirection = Vector2.right;

    [Header("Bouncing")]
    public int maxBounces = 1;

    [Tooltip("Extra upward angle added to the bounce on top of the normal reflection (degrees).")]
    public float bounceAngleBonus = 10f;

    [Header("Slowdown After Bounce")]
    public float slowdownRate = 5f;
    public float slowdownDuration = 1f;

    public bool HasHitEnemy { get; private set; } = false;

    Rigidbody2D rb;
    Collider2D myCollider;
    int bouncesUsed = 0;
    bool isSlowingDown = false;
    float slowdownTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        rb.linearVelocity = launchDirection.normalized * speed;
        Destroy(gameObject, lifetime);

        // Catch the case where we spawned already overlapping an enemy's collider.
        // OnTriggerEnter2D doesn't fire for pre-existing overlaps - only when something
        // ENTERS a collider - so we have to check manually on spawn.
        CheckInitialOverlaps();
    }

    void CheckInitialOverlaps()
    {
        if (myCollider == null) return;

        // Grab everything our collider currently overlaps
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true; // include trigger colliders in results
        Collider2D[] results = new Collider2D[10];
        int count = myCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            // Run the same logic OnTriggerEnter2D would
            HandleHit(results[i]);
            // Once we've bounced, no need to keep checking
            if (bouncesUsed >= maxBounces) return;
        }
    }

    private void FixedUpdate()
    {
        if (!isSlowingDown) return;

        slowdownTimer += Time.fixedDeltaTime;

        if (slowdownTimer >= slowdownDuration)
        {
            rb.linearVelocity = Vector2.zero;
            isSlowingDown = false;
            return;
        }

        Vector2 v = rb.linearVelocity;
        float drop = slowdownRate * Time.fixedDeltaTime;
        float currentSpeed = v.magnitude;
        float newSpeed = Mathf.Max(0f, currentSpeed - drop);

        if (currentSpeed > 0f)
        {
            rb.linearVelocity = v.normalized * newSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other);
    }

    void HandleHit(Collider2D other)
    {
        Damageable damageable = other.GetComponentInParent<Damageable>();
        if (damageable == null) return;

        HasHitEnemy = true;

        if (bouncesUsed >= maxBounces) return;

        // Reflect vertically
        Vector2 reflected = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);

        // Compute angle, add the upward bonus, rebuild vector
        float currentAngle = Mathf.Atan2(reflected.y, reflected.x) * Mathf.Rad2Deg;
        float adjustedAngle = currentAngle + (reflected.x >= 0f ? bounceAngleBonus : -bounceAngleBonus);
        float speedMagnitude = reflected.magnitude;
        float adjustedRad = adjustedAngle * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector2(Mathf.Cos(adjustedRad), Mathf.Sin(adjustedRad)) * speedMagnitude;

        bouncesUsed++;
        isSlowingDown = true;
        slowdownTimer = 0f;
    }
}