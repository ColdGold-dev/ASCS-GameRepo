using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    [SerializeField] private GameObject[] MasterScripts;
    public static GameMasterScript Instance { get; private set; }
    public PlayerMasterScript PlayerMasterScript { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensures only one exists
        }
        foreach (GameObject script in MasterScripts)
        {
            if (script == null)
            {
                Debug.LogError("One of the MasterScripts is not assigned in the inspector.");
            }
            else
            {
                //PlayerMasterScript = script.GetComponent<PlayerMasterScript>();
            }
        }

    }
    
    // Optional: Clear static reference when destroyed
    void OnDestroy() {
        if (Instance == this) Instance = null;
    }
}