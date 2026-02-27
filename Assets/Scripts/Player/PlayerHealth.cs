using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public GameObject[] lives;

    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;

    private GameObject gameOverMenu;

    [Header("Life Sprites")]
    public Sprite fullLife;
    public Sprite halfLife;
    public Sprite emptyLife;
    
    [Header("Music")]
    private MusicManager musicManager;
    [SerializeField] private AudioSource Main;
    [SerializeField] private AudioManager AM;

    private void Start()
    {
        maxHealth += upgradeStatsScriptable.health;

        Debug.Log(upgradeStatsScriptable.health);

        if(maxHealth <= 8)
        {
            int healthAmount = maxHealth / 2;

            if(maxHealth%2 != 0 )
            {
                healthAmount++;
            }

            for (int i = 0; i < healthAmount; i++)
            {
                lives[i].SetActive(true);            
            }

            if(maxHealth % 2 != 0) 
            { 
                lives[healthAmount-1].GetComponent<Image>().sprite = halfLife;
            }
        }

        currentHealth = maxHealth;

        AM = FindAnyObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();

        //gameOverMenu = UIManager.Instance.gameOverMenu;
        //musicManager = ReferenceManager.Instance.musicManager;
    }

    private void Die()
    {
        //Destroy(gameObject);
        Debug.Log("You died.");
        
        if (gameOverMenu)
        {
            gameOverMenu.SetActive(true);
        }

        if(musicManager) musicManager.SwitchToNewSongNoSave(2);
    }

    public void TakeDamage(int damage)
    {
        if (PlayerMovement.IsInvincible())
        {
            return;
        }

        currentHealth -= damage;
        CameraShake.Shake(0.2f, 0.2f);
        Debug.Log("Health: " +  currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        AM.Play("Hurt", Main);
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

                int heartHealth = currentHealth - (i * 2);

                if (heartHealth >= 2)
                {
                    lifeImage.sprite = fullLife;
                }
                else if (heartHealth == 1)
                {
                    lifeImage.sprite = halfLife;
                }
                else
                {
                    lifeImage.sprite = emptyLife;
                }
            }
        }
    }
}
