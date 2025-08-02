using UnityEngine;

public class CameraAdjust : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minSize = 6f;
    [SerializeField] private float maxSize = 8f;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.orthographicSize = minSize; // Set initial size
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void Update()
    {
        if (mainCam == null) return;

        float size = mainCam.orthographicSize;

        if (Input.GetKey(KeyCode.Q))
        {
            size += zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            size -= zoomSpeed * Time.deltaTime;
        }

        // Clamp and apply
        size = Mathf.Clamp(size, minSize, maxSize);
        mainCam.orthographicSize = size;
    }
}
