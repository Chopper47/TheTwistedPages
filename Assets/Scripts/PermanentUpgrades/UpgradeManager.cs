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
        currentUpgradeStats.PushPower = pushPowerUpgradeLv;
    }

    public void UpgradeHealth()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeHealth))
        {
            healthUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeHealth);

        }
    }
    public void UpgradeSpeed()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeSpeed))
        {
            speedUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeSpeed);

        }

    }
    public void UpgradeBulletTime()
    {
        if (currencyManager.CheckEnoughCurrencyAmount(costToUpgradeBulletTime))
        {
            bulletTimeUpgradeLv++;
            UpdateScriptableObjectStats();
            currencyManager.RemoveCurrency(costToUpgradeBulletTime);
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
