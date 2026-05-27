using UnityEngine;
using UnityEngine.InputSystem;

// Handles the teleport-to-knife + downward dive ability.
// Lives on the same GameObject as PlayerRangedAttack and reads its ActiveKnife.
[RequireComponent(typeof(PlayerRangedAttack))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TouchingDirections))]
public class PlayerDiveAttack : MonoBehaviour
{
    [Header("Dive")]
    [Tooltip("Downward speed the player slams at after teleporting")]
    public float diveSpeed = 20f;

    [Tooltip("Damage dealt to enemies touched during the dive")]
    public int diveDamage = 1;

    [Tooltip("Knockback applied to enemies hit during the dive")]
    public Vector2 diveKnockback = new Vector2(0f, 5f);

    [Tooltip("Radius around the player that damages enemies while diving")]
    public float diveDamageRadius = 0.6f;

    // References to other components on this GameObject
    PlayerRangedAttack rangedAttack;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;

    bool isDiving = false;

    private void Awake()
    {
        rangedAttack = GetComponent<PlayerRangedAttack>();
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        if (!isDiving) return;

        // End the dive when we land
        if (touchingDirections.IsGrounded)
        {
            isDiving = false;
            return;
        }

        // Damage any Damageable enemies in our dive hitbox
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, diveDamageRadius);
        foreach (Collider2D c in overlaps)
        {
            Damageable d = c.GetComponentInParent<Damageable>();
            if (d == null) continue;
            if (d.gameObject == gameObject) continue; // don't hit ourselves

            d.Hit(diveDamage, diveKnockback);
            // Damageable's own invincibility timer prevents spamming
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        // Ask PlayerRangedAttack what knife it has out
        GameObject knife = rangedAttack.ActiveKnife;
        if (knife == null) return;

        // Check the knife's state - only allow teleport if it has hit an enemy
        Projectile projectile = knife.GetComponent<Projectile>();
        if (projectile == null) return;
        if (!projectile.HasHitEnemy) return;

        // All clear - teleport and dive
        transform.position = knife.transform.position;
        Destroy(knife);

        rb.linearVelocity = new Vector2(0f, -diveSpeed);
        isDiving = true;
    }

    // Show the dive damage radius in the Scene view while tuning
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, diveDamageRadius);
    }
}
