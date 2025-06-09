using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform objectToTrack;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    private Camera thisCamera;
    private InputAction zoomAction;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
        
        // Set up zoom input action
        zoomAction = new InputAction(
            name: "Zoom",
            type: InputActionType.Value,
            binding: "<Mouse>/scroll"
        );
        zoomAction.Enable();
    }

    void LateUpdate()
    {
        if (objectToTrack != null)
        {
            transform.position = objectToTrack.position + offset;
        }

        // Read scroll input as Vector2 and use the y component
        Vector2 scrollValue = zoomAction.ReadValue<Vector2>();
        float scroll = scrollValue.y; // Vertical scroll amount
        
        if (scroll != 0)
        {
            float newSize = thisCamera.orthographicSize - scroll * zoomSpeed;
            thisCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    void OnDestroy()
    {
        zoomAction.Disable();
    }
}