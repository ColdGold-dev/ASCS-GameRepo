using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Input")]
    public InputActionReference dashAction;

    [Header("Trail")]
    public TrailRenderer trailRenderer;

    private Rigidbody2D rb;
    private bool canDash = true;
    private bool isDashing = false;

private Damageable damageable;

private void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    damageable = GetComponent<Damageable>();
}

    private void OnEnable()
    {
        dashAction.action.performed += OnDash;
        dashAction.action.Enable();
    }

    private void OnDisable()
    {
        dashAction.action.performed -= OnDash;
        dashAction.action.Disable();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

   private IEnumerator PerformDash()
{
    canDash = false;
    isDashing = true;

    // Disable gravity and lock velocity updates
    float originalGravity = rb.gravityScale;
    rb.gravityScale = 0f;

    if (damageable != null)
        damageable.LockVelocity = true;

    float direction = transform.localScale.x > 0 ? 1f : -1f;
    rb.linearVelocity = new Vector2(direction * dashingPower, 0f);

    if (trailRenderer != null)
        trailRenderer.emitting = true;

    yield return new WaitForSeconds(dashingTime);

    rb.gravityScale = originalGravity;
    if (trailRenderer != null)
        trailRenderer.emitting = false;

    isDashing = false;

    if (damageable != null)
        damageable.LockVelocity = false;

    yield return new WaitForSeconds(dashingCooldown);
    canDash = true;
}

}
