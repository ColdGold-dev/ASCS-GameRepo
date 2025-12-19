using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Move the player to the respawn point
            other.transform.position = respawnPoint.position;

            // Stop player velocity if they have a Rigidbody2D
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log("Player touched kill zone and was respawned.");
        }
    }
}
