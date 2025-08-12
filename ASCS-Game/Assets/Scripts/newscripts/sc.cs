using UnityEngine;

public class MoveLeftSlowly : MonoBehaviour
{
    public float moveSpeed = 1f; // Units per second

    void Update()
    {
        // Move left (negative X direction)
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}
