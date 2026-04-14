using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;
    private int extraHealth = 0;

    [Header("UI")]
    [SerializeField] private GameObject lifePrefab;
    [SerializeField] private Transform livesContainer;
    [SerializeField] private Sprite fullLife;
    [SerializeField] private Sprite halfLife;
    [SerializeField] private Sprite emptyLife;

    [SerializeField] private Sprite extraFullLife;
    [SerializeField] private Sprite extraHalfLife;

    private List<Image> lives = new List<Image>();

    [Header("Gameplay Settings")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;
    public bool inTutorial = false;
    public bool isHitStopping = false;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverMenu;

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    [SerializeField] private AudioSource Main;
    private MusicManager musicManager;

    [Header("VFX")]
    [SerializeField] private ParticleSystem healParticles;

    [Header("Other")]
    [SerializeField] private GameObject model;
    [SerializeField] private Animator animator;

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
        CameraShake.Shake(0.2f, 0.2f);

        PlayerUpgrades upgrades = GetComponent<PlayerUpgrades>();

        if (upgrades != null && upgrades.activeUpgrade != null)
        {
            if (upgrades.activeUpgrade.behaviour is ShieldUpgrade shield)
            {
                if (shield.TryBlockDamage(upgrades))
                {
                    return;
                }
            }
        }

        if (PlayerMovement.IsInvincible())
        {
            return;
        }

        if (!inTutorial)
        {
            int remainingDamage = damage;

            if (extraHealth > 0)
            {
                int used = Mathf.Min(extraHealth, remainingDamage);
                extraHealth -= used;
                remainingDamage -= used;
            }

            if (remainingDamage > 0 && !inTutorial)
            {
                currentHealth -= remainingDamage;
            }
        }

        if(model) StartCoroutine(DamageFlicker(invincibilityDuration, 0.05f));
        AM.Play("Hurt", Main);
        //StartCoroutine(HitStop(0.1f));

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

    IEnumerator HitStop(float duration)
    {
        if (isHitStopping) yield break;

        isHitStopping = true;

        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        isHitStopping = false;
        Time.timeScale = originalTimeScale;
    }

    IEnumerator DamageFlicker(float duration, float interval)
    {
        float timer = 0f;

        while (timer < duration)
        {
            model.SetActive(false);
            yield return new WaitForSeconds(interval);

            model.SetActive(true);
            yield return new WaitForSeconds(interval);

            timer += interval * 2;
        }

        model.SetActive(true);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        healParticles.Play();

        UpdateHealthUI();
    }

    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        PlayerMovement playerMovement = ReferenceManager.Instance.playerMovement;
        PlayerMovement.canMove = false;
        playerMovement.canRoll = false;

        if (gameOverMenu)
        {
            //gameOverMenu.SetActive(true);
        }

        if (musicManager)
        {
            musicManager.SwitchToNewSongNoSave(2);
        }
    }

    private IEnumerator TemporaryInvincibility()
    {
        PlayerMovement.isInvincible = true;

        yield return new WaitForSeconds(invincibilityDuration);

        PlayerMovement.isInvincible = false;
    }

    public void AddExtraLife(int amount)
    {
        extraHealth += amount;

        int normalHearts = Mathf.CeilToInt(maxHealth / 2.0f);
        int extraHeartsNeeded = Mathf.CeilToInt(extraHealth / 2.0f);
        int currentExtraHearts = lives.Count - normalHearts;

        int heartsToAdd = extraHeartsNeeded - currentExtraHearts;

        for (int i = 0; i < heartsToAdd; i++)
        {
            GameObject heart = Instantiate(lifePrefab, livesContainer);
            lives.Add(heart.GetComponent<Image>());
        }

        UpdateHealthUI();
    }
    #endregion

    #region UI
    private void SetUpHealthUI()
    {
        if (livesContainer == null)
        {
            return;
        }

        foreach (Transform child in livesContainer)
        {
            Destroy(child.gameObject);
        }

        lives.Clear();

        int normalHearts = Mathf.CeilToInt(maxHealth / 2.0f);
        int extraHearts = Mathf.CeilToInt(extraHealth / 2.0f);

        for (int i = 0; i < normalHearts; i++)
        {
            GameObject heart = Instantiate(lifePrefab, livesContainer);
            lives.Add(heart.GetComponent<Image>());
        }

        for (int i = 0; i < extraHearts; i++)
        {
            GameObject heart = Instantiate(lifePrefab, livesContainer);
            lives.Add(heart.GetComponent<Image>());
        }
    }

    private void UpdateHealthUI()
    {
        if (livesContainer == null)
        {
            return;
        }

        int normalHearts = Mathf.CeilToInt(maxHealth / 2.0f);
        int extraHearts = Mathf.CeilToInt(extraHealth / 2.0f);

        for (int i = 0; i < normalHearts; i++)
        {
            Image lifeImage = lives[i];

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

        for (int i = lives.Count - 1; i >= normalHearts; i--)
        {
            Image lifeImage = lives[i];

            int heartIndex = i - normalHearts;
            int heartHealth = extraHealth - (heartIndex * 2);

            if (heartHealth >= 2)
            {
                lifeImage.sprite = extraFullLife;
                lifeImage.gameObject.SetActive(true);
            }
            else if (heartHealth == 1)
            {
                lifeImage.sprite = extraHalfLife;
                lifeImage.gameObject.SetActive(true);
            }
            else
            {
                Destroy(lifeImage.gameObject);
                lives.RemoveAt(i);
            }
        }
    }
    #endregion
}
