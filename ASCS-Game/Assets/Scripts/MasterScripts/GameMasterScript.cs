using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    // Singleton pattern (thread-safe via null check)
    public static GameMasterScript Instance { get; private set; }

    // Sub-controller references (all private, exposed via properties)
    [SerializeField] private PlayerController _playerController;
    //[SerializeField] private AudioController _audioController;
    //[SerializeField] private SceneController _sceneController;
    //[SerializeField] private GameStateController _gameStateController;

    // Public accessors (read-only)
    public PlayerController Player => _playerController;
    //public AudioController Audio => _audioController;
    //public SceneController Scene => _sceneController;
    //public GameStateController GameState => _gameStateController;

    private void Awake()
    {
        // Handle singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        // Optional: Initialize sub-controllers if they need the GameMaster reference
        Initialize();
    }

    private void Initialize()
    {
        // Example: Inject GameMaster into sub-controllers if needed
        // _playerController.Initialize(this);
        // _audioController.Initialize(this);
    }

    // Optional: Cleanup when destroyed
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}