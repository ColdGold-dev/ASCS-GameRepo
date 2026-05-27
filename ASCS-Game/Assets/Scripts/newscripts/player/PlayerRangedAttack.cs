using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Playermovement))]
public class PlayerRangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform launchPoint;

    [Header("Aerial Aiming Arc")]
    public float upAngle = 12f;
    public float downAngle = -35f;
    public float speedRange = 10f;

    Playermovement playerMovement;
    Rigidbody2D playerRb;
    TouchingDirections touchingDirections;

    // The knife currently in the air. Null when no knife exists.
    GameObject activeKnife;

    // Exposed for PlayerDiveAttack to read.
    // Unity's null check handles destroyed objects automatically.
    public GameObject ActiveKnife => activeKnife;

    private void Awake()
    {
        playerMovement = GetComponent<Playermovement>();
        playerRb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (projectilePrefab == null) return;

        // If a knife is already in the air, this script does nothing.
        // - If the knife is bounced/armed, PlayerDiveAttack handles the press.
        // - If the knife isn't armed yet, the press is just ignored (wait for it).
        if (activeKnife != null) return;

        ThrowKnife();
    }

    void ThrowKnife()
    {
        Vector3 spawnPosition = launchPoint != null ? launchPoint.position : transform.position;
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        float angleDegrees;
        if (touchingDirections.IsGrounded)
        {
            angleDegrees = 0f;
        }
        else
        {
            float yVel = playerRb.linearVelocity.y;
            float t = Mathf.InverseLerp(-speedRange, speedRange, yVel);
            angleDegrees = Mathf.Lerp(downAngle, upAngle, t);
        }

        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        float horizontal = playerMovement.IsFacingRight ? 1f : -1f;
        Vector2 direction = new Vector2(Mathf.Cos(angleRadians) * horizontal, Mathf.Sin(angleRadians));

        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.launchDirection = direction;
        }

        newProjectile.transform.localScale = new Vector3(horizontal, 1f, 1f);

        // Store the reference so PlayerDiveAttack can find it later
        activeKnife = newProjectile;
    }
}