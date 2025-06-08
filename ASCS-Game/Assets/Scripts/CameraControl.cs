using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Drag your player object here")]
    [SerializeField] Transform objectToTrack;
    [SerializeField] Vector3 offset;
    void LateUpdate()
    {
        if(objectToTrack != null)
        {
            transform.position = objectToTrack.position + offset;
        }
    }
}