using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    #region Events
    [Header("Detection Events")]
    public UnityEvent noCollidersRemain = new UnityEvent();
    public UnityEvent playerDetected = new UnityEvent();
    public UnityEvent playerLost= new UnityEvent();
    #endregion
    public bool IsEmpty => detectedColliders.Count == 0;

    #region Detection State
    public List<Collider2D> detectedColliders;
    public bool IsPlayerDetected { get; private set; }
    private bool wasPlayerDetected = false;
    #endregion

    #region Vision Parameters
    [Header("Vision Parameters")]
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private float visionDistance = 5f;
    [SerializeField] private float minimDetectionDistance = 1f;
    #endregion

    #region Cached References
    private Collider2D col;
    private Vector3 playerPos => GameMasterScript.Instance.Player.GetPlayerPosition();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        detectedColliders = new List<Collider2D>();
    }

    private void Update()
    {
        EnemyVision();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
  //      Debug.Log("Entered: " + collision.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);

        if (detectedColliders.Count <= 0)
        {
            noCollidersRemain.Invoke();
        }
    }
    #endregion

    #region Detection Logic
    // Check if a player is within a cone vision, uses math only
    private void EnemyVision()
    {
        Vector2 toPlayer = playerPos - transform.position;
        float angleToPlayer = Vector2.Angle(transform.right, toPlayer);

        if (angleToPlayer <= visionAngle * 0.5f && toPlayer.magnitude <= visionDistance)
        {
       //     Debug.Log(this.name + "|| Player in cone!");
            playerDetected.Invoke();
            wasPlayerDetected = true;
        }
        // Check if a player is within a close range, uses distance check (simple circle check)
        else if (toPlayer.magnitude <= minimDetectionDistance)
        {
       //     Debug.Log(this.name + "|| Player is within 1 unit radius of enemy!");
            playerDetected.Invoke();
            wasPlayerDetected = true;
        }
        else if (wasPlayerDetected)
        {
     //       Debug.Log(this.name + "|| Player lost!");
            playerLost.Invoke();
            wasPlayerDetected = false;
        }
    }
    #endregion
}
