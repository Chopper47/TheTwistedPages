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
                TakeDamage();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            if (!playerMovement.IsInvincible())
            {
                playerMovement.canMove = false;
                TakeDamage();

                GetComponent<PlayerFall>().FallIntoHole(new Vector3(other.transform.position.x, other.transform.position.y - 5, other.transform.position.z));
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("You died.");
    }

    private void TakeDamage()
    {
        health--;
        Debug.Log("Health: " +  health);
        // UI hearts logic.
    }
}
