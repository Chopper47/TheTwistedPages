using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    private PlayerMovement playerMovement;

    public GameObject[] lives;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            if (!playerMovement.IsInvincible())
            {
                Trap trap = other.GetComponent<Trap>();
                TakeDamage(trap.GetDamage());
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
                Trap trap = other.GetComponent<Trap>();
                
                TakeDamage(trap.GetDamage());

                GetComponent<PlayerFall>().FallIntoHole(new Vector3(other.transform.position.x, other.transform.position.y - 5, other.transform.position.z));
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("You died.");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Health: " +  currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        Debug.Log("Health: " + currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < lives.Length; i++)
        {
            Image lifeImage = lives[i].GetComponent<Image>();

            if (currentHealth > i)
            {
                lifeImage.color = Color.white;
            }

            else
            {
                lifeImage.color = Color.black;
            }
        }
    }
}
