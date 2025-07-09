using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Range(0f, 1f)] private float parallaxFactor = 0.5f;
    private Vector3 previousPlayerPosition;

    void Start()
    {
        if (player != null)
        {
            previousPlayerPosition = player.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 deltaMovement = player.position - previousPlayerPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0f);
            previousPlayerPosition = player.position;
        }
    }
}
