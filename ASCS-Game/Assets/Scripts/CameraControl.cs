using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The transform of the player to follow")]
    Transform playerTransform;
    void Update()
    {
        this.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, this.transform.position.z);
    }
}
