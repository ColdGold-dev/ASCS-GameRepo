using UnityEngine;

public class PlayerMasterScript : MonoBehaviour
{
    protected static PlayerMasterScript Instance { get; private set; }
    [Header("GameObject References")]
    [SerializeField] private GameObject playerPrefab; // Reference to the player prefab

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    

    }

    // Optional: Clear static reference when destroyed
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public GameObject GetPlayerPrefab()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned in the inspector.");
            return null;
        }
        return playerPrefab;
    }

}
