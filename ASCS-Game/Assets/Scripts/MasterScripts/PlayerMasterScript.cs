using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMasterScript : MonoBehaviour
{
    public Item testItem;
    public Inventory inventory;

    public static PlayerMasterScript Instance { get; private set; }

    [Header("GameObject References")]
    [SerializeField] private GameObject playerPrefab;
    private Vector3 playerPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (playerPrefab != null)
        {
            playerPosition = playerPrefab.transform.position;
        }
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
{
    Equipment eq = inventory != null ? inventory.GetComponent<Equipment>() : null;
    if (eq != null && testItem != null)
    {
        eq.Equip(testItem);
    }
}
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
{
             Equipment eq = inventory != null ? inventory.GetComponent<Equipment>() : null;
             if (eq != null && testItem != null)
             {
                eq.Equip(testItem);
             }
}
        // Press P to add a test item to inventory
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (inventory != null && testItem != null)
            {
                inventory.AddItem(testItem, 1);
                Debug.Log("Total potions: " + inventory.CountOf(testItem));
            }
            else
            {
                Debug.Log("P pressed but inventory or testItem not assigned in Inspector");
            }
        }
    }

    public GameObject GetPlayerPrefab()
    {
        if (playerPrefab == null) return null;
        return playerPrefab;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }
}