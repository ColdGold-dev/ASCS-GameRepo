using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private float health = 1000f;

    //private GameMasterScript myGameMaster; // Reference to the GameMasterScript
    private float maxHealth;

    Transform healthBarTransform;
    private void Awake()
    {
        maxHealth = health; // Store the initial health as max health
        if (healthBar != null)
        {
            healthBarTransform = healthBar.transform;
            healthBar.SetActive(false); // Initially hide the health bar
        }
        else
        {
            Debug.LogError("Health bar is not assigned in the inspector.");
        }
        //myGameMaster = GameMasterScript.Instance; // Get the GameMasterScript instance
    }
    // Update is called once per frame

    void Start()
    {
        TakeDamage(100f);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            healthBar.SetActive(true); // Show health bar when taking damage
            ShowHealthBar();
            print("Enemy took damage: " + damage + ", remaining health: " + health);
        }

    }

    private void ShowHealthBar()
    {
        if (healthBar != null)
        {
            float healthPercentage = health / maxHealth;
            healthBarTransform.localScale = new Vector3(healthPercentage, healthBarTransform.localScale.y, healthBarTransform.localScale.z);
        }
        else
        {
            Debug.LogError("Health bar not found!");
        }
    }
    void Die()
    {
        print("Enemy died");
        Destroy(gameObject); // Destroy the enemy game object
    }

}
