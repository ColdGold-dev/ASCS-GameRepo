using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;

    [Header("Dash Hit (when dashing into an enemy)")]
    public int dashHitDamage = 25;
    public Vector2 dashHitKnockback = new Vector2(6f, 4f);
    [Tooltip("How hard the player bounces up when the dash hits an enemy")]
    public float dashBounceImpulse = 12f;

    [Header("Input")]
    public InputActionReference dashAction;

    [Header("Trail")]
    public TrailRenderer trailRenderer;

    private Rigidbody2D rb;
    private Damageable damageable;
    private bool isDashing = false;

    // Dash starts locked. PlayerParry grants a charge on parry success.
    private bool hasDashCharge = false;

    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        if (dashAction != null)
        {
            dashAction.action.performed += OnDash;
            dashAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
        {
            dashAction.action.performed -= OnDash;
            dashAction.action.Disable();
        }
    }

    // Called externally (e.g. by PlayerParry when a parry succeeds)
    public void GrantDashCharge()
    {
        hasDashCharge = true;
        Debug.Log("DASH | charge granted");
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (isDashing) return;
        if (!hasDashCharge)
        {
            Debug.Log("DASH | no charge - dash not allowed");
            return;
        }

        hasDashCharge = false;
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if (damageable != null) damageable.LockVelocity = true;

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * dashingPower, 0f);

        if (trailRenderer != null) trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        // Restore physics
        rb.gravityScale = originalGravity;
        if (trailRenderer != null) trailRenderer.emitting = false;
        if (damageable != null) damageable.LockVelocity = false;

        isDashing = false;
    }

    // Detect dashing-into-enemy hits.
    // Requires this GameObject (or a child) to have a collider that overlaps with the enemy.
    // The Player's main Collider2D is usually fine.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDashing) return;
        TryDashHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;
        TryDashHit(collision.gameObject);
    }

    private void TryDashHit(GameObject other)
    {
        Damageable target = other.GetComponentInParent<Damageable>();
        if (target == null) return;
        if (target.gameObject == gameObject) return; // don't hit ourselves

        // Deal damage
        target.Hit(dashHitDamage, dashHitKnockback);
        Debug.Log("DASH HIT | " + target.gameObject.name + " for " + dashHitDamage);

        // Bounce the player up
        rb.gravityScale = 1f; // restore gravity so the bounce arcs back down naturally
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.3f, dashBounceImpulse);

        // End the dash early
        StopAllCoroutines();
        if (trailRenderer != null) trailRenderer.emitting = false;
        if (damageable != null) damageable.LockVelocity = false;
        isDashing = false;
    }
}