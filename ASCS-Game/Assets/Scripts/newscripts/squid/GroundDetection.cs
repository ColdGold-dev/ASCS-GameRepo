using UnityEngine;
using UnityEngine.Events;

public class GroundDetection : MonoBehaviour
{
    [Tooltip("Called when the object is no longer touching any ground.")]
    public UnityEvent onCliffDetected;

    private int groundContacts = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name}: ENTERED collider {other.name} of type {other.GetType()}");

        if (other.CompareTag("Ground"))
        {
            groundContacts++;
            Debug.Log($"{name}: Ground contact +1 (total: {groundContacts})");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"{name}: EXITED collider {other.name} of type {other.GetType()}");

        if (other.CompareTag("Ground"))
        {
            groundContacts--;
            Debug.Log($"{name}: Ground contact -1 (total: {groundContacts})");

            if (groundContacts <= 0)
            {
                Debug.Log($"{name}: No more ground! Emitting event.");
                onCliffDetected?.Invoke();
            }
        }
    }
}
