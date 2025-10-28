using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public GameObject[] lives;

    [SerializeField] private float invincibilityDuration = 0.5f;

    public GameObject gameOverMenu;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            if (!PlayerMovement.IsInvincible())
            {
                Trap trap = other.GetComponent<Trap>();
                TakeDamage(trap.GetDamage());
            }
        }

        if (other.CompareTag("Hole"))
        {
            if (!PlayerMovement.IsInvincible())
            {
                PlayerMovement.canMove = false;
                Trap trap = other.GetComponent<Trap>();
                
                TakeDamage(trap.GetDamage());

                GetComponent<PlayerFall>().FallIntoHole();
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("You died.");
        gameOverMenu.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Health: " +  currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
        StartCoroutine(TemporaryInvincibility());
    }

    private IEnumerator TemporaryInvincibility()
    {
        PlayerMovement.isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        PlayerMovement.isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        Debug.Log("Health: " + currentHealth);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < lives.Length; i++)
        {
            if (lives[i] != null)
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
}
