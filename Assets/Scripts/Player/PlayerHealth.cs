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
    private MusicManager music;

    private void Start()
    {
        currentHealth = maxHealth;
        music = FindFirstObjectByType<MusicManager>();
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("You died.");
        
        if (gameOverMenu)
        {
            gameOverMenu.SetActive(true);
        }

        //music.SwitchToNewSongNoSave(2);
    }

    public void TakeDamage(int damage)
    {
        if (PlayerMovement.IsInvincible())
        {
            return;
        }

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
