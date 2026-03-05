using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;

    [Header("UI")]
    [SerializeField] private GameObject[] lives;
    [SerializeField] private Sprite fullLife;
    [SerializeField] private Sprite halfLife;
    [SerializeField] private Sprite emptyLife;

    [Header("Gameplay Settings")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;
    public bool inTutorial = false;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverMenu;

    [Header("Audio")]
    [SerializeField] private AudioManager AM;
    [SerializeField] private AudioSource Main;
    private MusicManager musicManager;

    private void Start()
    {
        InitializeHealth();
        InitializeAudio();
        UpdateHealthUI();
    }

    #region Initialization
    private void InitializeHealth()
    {
        maxHealth += upgradeStatsScriptable.health;
        currentHealth = maxHealth;
        SetUpHealthUI();
    }

    private void InitializeAudio()
    {
        AM = FindAnyObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();

        //musicManager = ReferenceManager.Instance.musicManager;
    }
    #endregion

    #region Health Management
    public void TakeDamage(int damage)
    {
        if (PlayerMovement.IsInvincible())
        {
            return;
        }

        if (!inTutorial)
        {
            currentHealth -= damage;
        }

        CameraShake.Shake(0.2f, 0.2f);
        AM.Play("Hurt", Main);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(TemporaryInvincibility());
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        UpdateHealthUI();
    }

    private void Die()
    {
        Debug.Log("You died.");

        if (gameOverMenu)
        {
            gameOverMenu.SetActive(true);
        }

        if (musicManager)
        {
            musicManager.SwitchToNewSongNoSave(2);
        }

        Destroy(gameObject);
    }

    private IEnumerator TemporaryInvincibility()
    {
        PlayerMovement.isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        PlayerMovement.isInvincible = false;
    }
    #endregion

    #region UI
    private void SetUpHealthUI()
    {
        if (maxHealth <= 8)
        {
            int hearts = maxHealth / 2;

            if (maxHealth % 2 != 0)
            {
                hearts++;
            }

            for (int i = 0; i < hearts; i++)
            {
                lives[i].SetActive(true);

                if (maxHealth % 2 != 0 && i == hearts - 1)
                {
                    lives[i].GetComponent<Image>().sprite = halfLife;
                }
            }
        }
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
    #endregion
    
}
