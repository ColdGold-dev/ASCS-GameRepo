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

    // Other scripts (like PlayerDiveAttack) read this to know if the knife
    // is "armed" for teleport. Stays false until the knife touches an enemy.
    public bool HasHitEnemy { get; private set; } = false;

    Rigidbody2D rb;
    int bouncesUsed = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = launchDirection.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Damageable damageable = other.GetComponentInParent<Damageable>();
        if (damageable == null) return;

        // Mark this knife as armed for teleport. We set this BEFORE the bounce
        // check so even on the final allowed hit, the flag still flips.
        HasHitEnemy = true;

        if (bouncesUsed >= maxBounces) return;

        // Reflect vertically
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
        bouncesUsed++;
    }
}