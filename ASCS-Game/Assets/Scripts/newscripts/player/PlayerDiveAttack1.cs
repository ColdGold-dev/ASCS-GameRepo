using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerRangedAttack))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TouchingDirections))]
public class PlayerDiveAttack : MonoBehaviour
{
    [Header("Dive")]
    public float diveSpeed = 20f;
    public int diveDamage = 1;
    public Vector2 diveKnockback = new Vector2(0f, 2f);
    public float hangTime = 1f;

    [Header("Bounce on Hit")]
    public float bounceImpulse = 7f;

    PlayerRangedAttack rangedAttack;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Collider2D myCollider;

    bool isDiving = false;
    bool isHanging = false;
    bool hasBounced = false;
    float originalGravityScale;
    float diveStartTime;

    private void Awake()
    {
        rangedAttack = GetComponent<PlayerRangedAttack>();
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        myCollider = GetComponent<Collider2D>();
        originalGravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        if (!isDiving) return;
        if (Time.time - diveStartTime < 0.1f) return;

        if (touchingDirections.IsGrounded)
        {
            isDiving = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamageAndBounce(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageAndBounce(other);
    }

    void TryDamageAndBounce(Collider2D hit)
    {
        if (!isDiving) return;
        if (hasBounced) return;

        Damageable d = hit.GetComponentInParent<Damageable>();
        if (d == null) return;
        if (d.gameObject == gameObject) return;

        hasBounced = true;
        isDiving = false;

        d.Hit(diveDamage, diveKnockback);

        Rigidbody2D enemyRb = d.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            enemyRb.linearVelocity = Vector2.zero;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceImpulse);
    }

    // Checks what we're already overlapping with. Used right after teleport,
    // since the dive might land us already inside an enemy's collider, and Unity's
    // collision events only fire on ENTER, not on pre-existing overlaps.
    void CheckInitialOverlaps()
    {
        if (myCollider == null) return;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] results = new Collider2D[10];
        int count = myCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            TryDamageAndBounce(results[i]);
            if (hasBounced) return; // already handled, don't keep checking
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (isHanging || isDiving) return;

        GameObject knife = rangedAttack.ActiveKnife;
        if (knife == null) return;

        Projectile projectile = knife.GetComponent<Projectile>();
        if (projectile == null || !projectile.HasHitEnemy) return;

        transform.position = knife.transform.position;
        Destroy(knife);

        StartCoroutine(HangThenDive());
    }

    IEnumerator HangThenDive()
    {
        isHanging = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(hangTime);

        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = new Vector2(0f, -diveSpeed);
        isHanging = false;
        isDiving = true;
        hasBounced = false;
        diveStartTime = Time.time;

        // If we're already overlapping an enemy when the dive begins, handle it now
        // (Unity won't fire OnCollisionEnter for collisions that already started)
        CheckInitialOverlaps();
    }
}