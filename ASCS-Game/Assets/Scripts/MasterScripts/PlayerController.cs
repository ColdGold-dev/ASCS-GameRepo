using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _player; // Assign the player GameObject

    //Should add funtion to auto get player from inspector, SerializeField causing problems
    // Public getters
    public Vector3 GetPlayerPosition() => _player.transform.position;
    public GameObject GetPlayer() => _player;
}