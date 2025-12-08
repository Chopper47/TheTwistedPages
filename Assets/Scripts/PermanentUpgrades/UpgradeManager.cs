using TMPro;
using UnityEngine;


[RequireComponent(typeof(CurrencyManager))]
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradeStats currentUpgradeStats;

    [Header("Upgrade Levels")]
    [SerializeField] private int healthUpgradeLv;
    [SerializeField] private int speedUpgradeLv;
    [SerializeField] private int bulletTimeUpgradeLv;
    [SerializeField] private int pushPowerUpgradeLv;


    [Header("Currency Necessary")]
    [SerializeField] private int costToUpgradeHealth;
    [SerializeField] private int costToUpgradeSpeed;
    [SerializeField] private int costToUpgradeBulletTime;
    [SerializeField] private int costToUpgradePushPower;


    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI bulletTimeText;


    [SerializeField] private CurrencyManager currencyManager;
    private void Awake()
    {
        currencyManager = gameObject.GetComponent<CurrencyManager>();
    }
    private void Start()
    {
        UpdateScriptableObjectStats();
    }

    private void UpdateScriptableObjectStats()
    {
        currentUpgradeStats.health = healthUpgradeLv;
        currentUpgradeStats.speed = speedUpgradeLv;
        currentUpgradeStats.bulletTime = bulletTimeUpgradeLv;
        currentUpgradeStats.pushPower = pushPowerUpgradeLv;
    }

    public void UpgradeHealth()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeHealth))
        {
            healthUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeHealth);
            if(hpText != null) hpText.text = "Level: " + healthUpgradeLv;
        }
    }
    public void UpgradeSpeed()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeSpeed))
        {
            speedUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeSpeed);
            if (speedText != null) speedText.text = "Level: " + speedUpgradeLv;

        }

    }
    public void UpgradeBulletTime()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeBulletTime))
        {
            bulletTimeUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeBulletTime);
            if (bulletTimeText != null) bulletTimeText.text = "Level: " + bulletTimeUpgradeLv;

        }

    }
    public void UpgradePushPower()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradePushPower))
        {
            pushPowerUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradePushPower);

        }

    }

    

}
