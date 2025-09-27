using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        health = maxHealth;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            if (!playerMovement.IsInvincible())
            {
                health--;
                UpdateUI();
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("You died.");
    }

    private void UpdateUI()
    {
        Debug.Log("Health: " +  health);
        // UI hearts logic.
    }
}
