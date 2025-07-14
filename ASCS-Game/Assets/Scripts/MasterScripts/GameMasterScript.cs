using UnityEngine;

public class GameMasterScript : MonoBehaviour
{
    // Reference to all sub-controllers
    // Uncomment the controllers should be implemented

    //[SerializeField] private AudioController audioController;
    //[SerializeField] private SceneController sceneController;
    //[SerializeField] private GameStateController gameStateController;
    
    // Singleton pattern for easy access
    public static GameMasterScript Instance { get; private set; }

    // Public accessors for sub-controllers

    // Should manage the audio system
    // This controller will manage the audio system, such as background music, sound effects, and volume control
    //public AudioController Audio => audioController;

    // Should manage the scene loading and transitions
    // This controller will manage the scene transitions, loading, and unloading enemies, and other scene-related objects
    //public SceneController Scene => sceneController;

    //Should manage the game state
    // This controller will manage the game state, such as starting, pausing, and ending
    //public GameStateController GameState => gameStateController;

    private void Awake()
    {
        // Singleton implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        Initialize();
    }
    
    private void Initialize()
    {
        // Initialize all sub-controllers
        //audioController.Initialize(this);
        //sceneController.Initialize(this);
        //gameStateController.Initialize(this);
    }
}