using TMPro;
using UnityEngine;


[RequireComponent(typeof(CurrencyManager))]
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradeStats currentUpgradeStats;

    [Header("Upgrade Levels")]
    [SerializeField] private int healthUpgradeLv;
    [SerializeField] private int speedUpgradeLv;
    [SerializeField] private int roomChanceUpgradeLv;
    [SerializeField] private int currencyUpgradeLv;
    [SerializeField] private int dashTimeUpgradeLv;


    [Header("Currency Necessary")]
    [SerializeField] private int costToUpgradeHealth;
    [SerializeField] private int costToUpgradeSpeed;
    [SerializeField] private int costToUpgradeRoomChance;
    [SerializeField] private int costToUpgradeCurrency;
    [SerializeField] private int costToUpgradeDashTime;


    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI bulletTimeText;
    [SerializeField] private TextMeshProUGUI roomChanceText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI dashTimeText;


    [SerializeField] private CurrencyManager currencyManager;
    private void Awake()
    {
        currencyManager = gameObject.GetComponent<CurrencyManager>();
        UpdateScriptableObjectStats();
    }
    private void Start()
    {
    }

    private void UpdateScriptableObjectStats()
    {
        currentUpgradeStats.health = healthUpgradeLv;
        currentUpgradeStats.speed = speedUpgradeLv;
        currentUpgradeStats.roomChance = roomChanceUpgradeLv;
        currentUpgradeStats.currency = currencyUpgradeLv;
        currentUpgradeStats.dashTime = dashTimeUpgradeLv;

    }

    public void UpgradeHealth()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeHealth) && healthUpgradeLv < 5)
        {
            healthUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeHealth);
            if(SaveManager.instance != null)
            {
                SaveManager.instance.player.healthUpgradeLevel = healthUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }

            if (hpText != null) hpText.text = "Level: " + healthUpgradeLv;
        }
        else
        {
            Debug.Log("Not enough currency to upgrade health or max level reached.");
        }
    }
    public void UpgradeSpeed()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeSpeed) && speedUpgradeLv < 5)
        {
            speedUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeSpeed);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.speedUpgradeLevel = speedUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
            if (speedText != null) speedText.text = "Level: " + speedUpgradeLv;

        }

    }
    public void UpgradeRoomChance()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeRoomChance) && roomChanceUpgradeLv < 5)
        {
            roomChanceUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeRoomChance);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.roomChanceUpgradeLevel = roomChanceUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
            if (roomChanceText != null) roomChanceText.text = "Level: " + roomChanceUpgradeLv;
        }
    }

    public void UpgradeCurrency()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeCurrency) && currencyUpgradeLv < 5)
        {
            currencyUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeCurrency);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.moneyUpgradeLevel = currencyUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
            if (currencyText != null) currencyText.text = "Level: " + currencyUpgradeLv;
        }
    }

    public void UpgradeDashTime()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeDashTime) && dashTimeUpgradeLv < 5)
        {
            dashTimeUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeDashTime);
            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.dashTimeUpgradeLevel = dashTimeUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
            if (dashTimeText != null) dashTimeText.text = "Level: " + dashTimeUpgradeLv;
        }
    }



}
