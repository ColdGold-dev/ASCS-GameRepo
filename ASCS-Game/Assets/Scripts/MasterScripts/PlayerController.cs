using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody2D _rb; // Assign in Inspector

    [Header("References")]
    [SerializeField] private GameObject _player; // Assign the player GameObject

    // Public getters
    public Vector3 GetPlayerPosition() => _player.transform.position;
    public GameObject GetPlayer() => _player;
}