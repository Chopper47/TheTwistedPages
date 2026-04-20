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

    [Header("Notches")]
    [SerializeField] private UpgradeNotchUI healthNotches;
    [SerializeField] private UpgradeNotchUI speedNotches;
    [SerializeField] private UpgradeNotchUI roomChanceNotches;
    [SerializeField] private UpgradeNotchUI currencyNotches;
    [SerializeField] private UpgradeNotchUI dashTimeNotches;

    [SerializeField] private CurrencyManager currencyManager;

    private void Awake()
    {
        currencyManager = gameObject.GetComponent<CurrencyManager>();

        UpdateScriptableObjectStats();
    }

    private void Start()
    {
        healthNotches.SetLevel(healthUpgradeLv);
        speedNotches.SetLevel(speedUpgradeLv);
        roomChanceNotches.SetLevel(roomChanceUpgradeLv);
        currencyNotches.SetLevel(currencyUpgradeLv);
        dashTimeNotches.SetLevel(dashTimeUpgradeLv);
    }

    private void UpdateScriptableObjectStats()
    {
        Debug.Log("CURRENCY" + currentUpgradeStats.currency);
        currentUpgradeStats.health = healthUpgradeLv;
        currentUpgradeStats.speed = speedUpgradeLv;
        currentUpgradeStats.roomChance = roomChanceUpgradeLv;
        currentUpgradeStats.currency = currencyUpgradeLv;
        Debug.Log("CURRENCY2" + currentUpgradeStats.currency);

        currentUpgradeStats.dashTime = dashTimeUpgradeLv;
    }

    public void UpgradeHealth()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeHealth) && healthUpgradeLv < 5)
        {
            healthUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeHealth);

            if (healthNotches != null)
            {
                healthNotches.SetLevel(healthUpgradeLv);
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.healthUpgradeLevel = healthUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
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

            if (speedNotches != null)
            {
                speedNotches.SetLevel(speedUpgradeLv);
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.speedUpgradeLevel = speedUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

    }

    public void UpgradeRoomChance()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeRoomChance) && roomChanceUpgradeLv < 5)
        {
            roomChanceUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeRoomChance);

            if (roomChanceNotches != null)
            {
                roomChanceNotches.SetLevel(roomChanceUpgradeLv);
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.roomChanceUpgradeLevel = roomChanceUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }
    }

    public void UpgradeCurrency()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeCurrency) && currencyUpgradeLv < 3)
        {
            currencyUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeCurrency);

            if (currencyNotches != null)
            {
                currencyNotches.SetLevel(currencyUpgradeLv);
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.moneyUpgradeLevel = currencyUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }
    }

    public void UpgradeDashTime()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeDashTime) && dashTimeUpgradeLv < 5)
        {
            dashTimeUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeDashTime);

            if (dashTimeNotches != null)
            {
                dashTimeNotches.SetLevel(dashTimeUpgradeLv);
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.dashTimeUpgradeLevel = dashTimeUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }
    }
}
