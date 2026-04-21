using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(CurrencyManager))]
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private UpgradeStats currentUpgradeStats;
    [SerializeField] private GameObject upgradeMenuUI;

    [Header("Upgrade Levels")]
    [SerializeField] private int healthUpgradeLv;
    [SerializeField] private int speedUpgradeLv;
    [SerializeField] private int roomChanceUpgradeLv;
    [SerializeField] private int currencyUpgradeLv;
    [SerializeField] private int dashTimeUpgradeLv;

    [Header("Currency Necessary")]
    [SerializeField] private int[] costToUpgradeHealth = new int[] { 5, 15, 30, 50 };
    [SerializeField] private int[] costToUpgradeSpeed = new int[] { 5, 15, 30, 50 };
    [SerializeField] private int[] costToUpgradeDashTime = new int[] { 5, 15, 30, 50 };
    [SerializeField] private int[] costToUpgradeCurrency = new int[] { 20, 50 };
    [SerializeField] private int[] costToUpgradeRoomChance = new int[] { 5, 15, 30, 50 };


    [Header("Currency Necessary UI References")]
    [SerializeField] private TextMeshProUGUI healthUpgradeCostText;
    [SerializeField] private TextMeshProUGUI speedUpgradeCostText;
    [SerializeField] private TextMeshProUGUI currencyUpgradeCostText;
    [SerializeField] private TextMeshProUGUI dashTimeUpgradeCostText;
    [SerializeField] private TextMeshProUGUI roomChanceUpgradeCostText;

    [Header("Notches")]
    [SerializeField] private UpgradeNotchUI healthNotches;
    [SerializeField] private UpgradeNotchUI speedNotches;
    [SerializeField] private UpgradeNotchUI roomChanceNotches;
    [SerializeField] private UpgradeNotchUI currencyNotches;
    [SerializeField] private UpgradeNotchUI dashTimeNotches;

    [Header("Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button roomChanceUpgradeButton;
    [SerializeField] private Button currencyUpgradeButton;
    [SerializeField] private Button dashTimeUpgradeButton;

    [SerializeField] private CurrencyManager currencyManager;

    

    private void Awake()
    {
        currencyManager = gameObject.GetComponent<CurrencyManager>();

        costToUpgradeHealth = new int[] { 5, 15, 30, 50 };
        costToUpgradeSpeed = new int[] { 5, 15, 30, 50 };
        costToUpgradeRoomChance = new int[] { 5, 15, 30, 50 };
        costToUpgradeDashTime = new int[] { 5, 15, 30, 50 };
        costToUpgradeCurrency = new int[] { 20, 50 };
        UpdateScriptableObjectStats();

    }

    private void Start()
    {
        if (SaveManager.instance)
        {
            SaveManager.instance.LoadPlayerData();
            currencyManager.currentCurrencyAmount = SaveManager.instance.player.currencyAmount;

            healthUpgradeLv = SaveManager.instance.player.healthUpgradeLevel;
            speedUpgradeLv = SaveManager.instance.player.speedUpgradeLevel;
            roomChanceUpgradeLv = SaveManager.instance.player.roomChanceUpgradeLevel;
            currencyUpgradeLv = SaveManager.instance.player.moneyUpgradeLevel;
            dashTimeUpgradeLv = SaveManager.instance.player.dashTimeUpgradeLevel;

            UpdateScriptableObjectStats();
        }

        SetNotchesAndCostsUI();

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
        Debug.Log("UPGRAADING HP");
        if (healthUpgradeLv < 4 && currencyManager.CheckEnoughCurrencyAmount(costToUpgradeHealth[healthUpgradeLv]))
        {
            currencyManager.RemoveCurrency(costToUpgradeHealth[healthUpgradeLv]);
            healthUpgradeLv++;
            UpdateScriptableObjectStats();

            if (healthNotches != null)
            {
                healthNotches.SetLevel(healthUpgradeLv + 1);
            }

            if (healthUpgradeCostText && healthUpgradeLv < 4) healthUpgradeCostText.text = costToUpgradeHealth[healthUpgradeLv].ToString();

            if (healthUpgradeLv == 4)
            {
                if (healthUpgradeCostText) healthUpgradeCostText.text = "";
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.healthUpgradeLevel = healthUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

        if (healthUpgradeLv == 4)
        {
            if (healthUpgradeCostText) healthUpgradeCostText.text = "";
        }
    }

    public void UpgradeSpeed()
    {
        if (speedUpgradeLv < 4 && currencyManager.CheckEnoughCurrencyAmount(costToUpgradeSpeed[speedUpgradeLv]))
        {
            currencyManager.RemoveCurrency(costToUpgradeSpeed[speedUpgradeLv]);
            speedUpgradeLv++;
            UpdateScriptableObjectStats();

            if (speedNotches != null)
            {
                speedNotches.SetLevel(speedUpgradeLv + 1);
            }

            if (speedUpgradeCostText && speedUpgradeLv < 4) speedUpgradeCostText.text = costToUpgradeSpeed[speedUpgradeLv].ToString();
            if (speedUpgradeLv == 4)
            {
                if (speedUpgradeCostText) speedUpgradeCostText.text = "";
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.speedUpgradeLevel = speedUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

        if (speedUpgradeLv == 4)
        {
            if (speedUpgradeCostText) speedUpgradeCostText.text = "";
        }

    }

    public void UpgradeRoomChance()
    {
        if (roomChanceUpgradeLv < 4 && currencyManager.CheckEnoughCurrencyAmount(costToUpgradeRoomChance[roomChanceUpgradeLv]))
        {
            currencyManager.RemoveCurrency(costToUpgradeRoomChance[roomChanceUpgradeLv]);
            roomChanceUpgradeLv++;
            UpdateScriptableObjectStats();

            if (roomChanceNotches != null)
            {
                roomChanceNotches.SetLevel(roomChanceUpgradeLv + 1);
            }

            if (roomChanceUpgradeCostText && roomChanceUpgradeLv < 4) roomChanceUpgradeCostText.text = costToUpgradeRoomChance[roomChanceUpgradeLv].ToString();
            if (roomChanceUpgradeLv == 4)
            {
                if (roomChanceUpgradeCostText) roomChanceUpgradeCostText.text = "";
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.roomChanceUpgradeLevel = roomChanceUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

        if (roomChanceUpgradeLv == 4)
        {
            if (roomChanceUpgradeCostText) roomChanceUpgradeCostText.text = "";
        }
    }

    public void UpgradeCurrency()
    {
        if (currencyUpgradeLv < 2 && currencyManager.CheckEnoughCurrencyAmount(costToUpgradeCurrency[currencyUpgradeLv]))
        {
            currencyManager.RemoveCurrency(costToUpgradeCurrency[currencyUpgradeLv]);
            currencyUpgradeLv++;
            UpdateScriptableObjectStats();

            if (currencyNotches != null)
            {
                currencyNotches.SetLevel(currencyUpgradeLv + 1);
            }

            if (currencyUpgradeCostText && currencyUpgradeLv < 2) currencyUpgradeCostText.text = costToUpgradeCurrency[currencyUpgradeLv].ToString();
            if (currencyUpgradeLv == 2)
            {
                if (currencyUpgradeCostText) currencyUpgradeCostText.text = "";
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.moneyUpgradeLevel = currencyUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

        if(currencyUpgradeLv == 2)
        {
            if (currencyUpgradeCostText) currencyUpgradeCostText.text = "";
        }
    }

    public void UpgradeDashTime()
    {
        if (dashTimeUpgradeLv < 4 && currencyManager.CheckEnoughCurrencyAmount(costToUpgradeDashTime[dashTimeUpgradeLv]))
        {
            currencyManager.RemoveCurrency(costToUpgradeDashTime[dashTimeUpgradeLv]);
            dashTimeUpgradeLv++;
            UpdateScriptableObjectStats();

            if (dashTimeNotches != null)
            {
                dashTimeNotches.SetLevel(dashTimeUpgradeLv + 1);
            }

            if (dashTimeUpgradeCostText && dashTimeUpgradeLv < 4) dashTimeUpgradeCostText.text = costToUpgradeDashTime[dashTimeUpgradeLv].ToString();
            if (dashTimeUpgradeLv == 4)
            {
                if (dashTimeUpgradeCostText) dashTimeUpgradeCostText.text = "";
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.player.dashTimeUpgradeLevel = dashTimeUpgradeLv;
                SaveManager.instance.player.currencyAmount = currencyManager.currentCurrencyAmount;
                SaveManager.instance.SavePlayerData();
            }
        }

        if (dashTimeUpgradeLv == 4)
        {
            if (dashTimeUpgradeCostText) dashTimeUpgradeCostText.text = "";
        }
    }

    void SetNotchesAndCostsUI()
    {
        if (healthNotches != null)
        {
            healthNotches.SetLevel(healthUpgradeLv + 1);
        }

        if (speedNotches != null)
        {
            speedNotches.SetLevel(speedUpgradeLv + 1);
        }

        if (roomChanceNotches != null)
        {
            roomChanceNotches.SetLevel(roomChanceUpgradeLv + 1);
        }

        if (currencyNotches != null)
        {
            currencyNotches.SetLevel(currencyUpgradeLv + 1);
        }

        if (dashTimeNotches != null)
        {
            dashTimeNotches.SetLevel(dashTimeUpgradeLv + 1);
        }



        if (healthUpgradeCostText) healthUpgradeCostText.text = costToUpgradeHealth[healthUpgradeLv].ToString();
        if (speedUpgradeCostText) speedUpgradeCostText.text = costToUpgradeSpeed[speedUpgradeLv].ToString();
        if (roomChanceUpgradeCostText) roomChanceUpgradeCostText.text = costToUpgradeRoomChance[roomChanceUpgradeLv].ToString();
        if (currencyUpgradeCostText) currencyUpgradeCostText.text = costToUpgradeCurrency[currencyUpgradeLv].ToString();
        if (dashTimeUpgradeCostText) dashTimeUpgradeCostText.text = costToUpgradeDashTime[dashTimeUpgradeLv].ToString();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Gets all references when entering each scene, and loads the player data to update the stats and currency amount. Also updates the UI with the new values.
        if (scene.name == "Hub")
        {
            if (SaveManager.instance)
            {
                SaveManager.instance.LoadPlayerData();
                currencyManager.currentCurrencyAmount = SaveManager.instance.player.currencyAmount;
                currencyManager.UpdateUI();
                healthUpgradeLv = SaveManager.instance.player.healthUpgradeLevel;
                speedUpgradeLv = SaveManager.instance.player.speedUpgradeLevel;
                roomChanceUpgradeLv = SaveManager.instance.player.roomChanceUpgradeLevel;
                currencyUpgradeLv = SaveManager.instance.player.moneyUpgradeLevel;
                dashTimeUpgradeLv = SaveManager.instance.player.dashTimeUpgradeLevel;
                UpdateScriptableObjectStats();
            }

            upgradeMenuUI = GameObject.FindGameObjectWithTag("UpgradeMenu");
            currencyManager.pauseMenuCurrencyText = GameObject.Find("FlowerAmount").GetComponent<TextMeshProUGUI>();
            currencyManager.upgradeMenuCurrencyText = GameObject.Find("FlowerAmountMenu").GetComponent<TextMeshProUGUI>();
            currencyManager.UpdateUI();



            healthUpgradeCostText = GameObject.Find("FlowerAmountHealth").GetComponent<TextMeshProUGUI>();
            speedUpgradeCostText = GameObject.Find("FlowerAmountSpeed").GetComponent<TextMeshProUGUI>();
            roomChanceUpgradeCostText = GameObject.Find("FlowerAmountCurrency").GetComponent<TextMeshProUGUI>();
            currencyUpgradeCostText = GameObject.Find("FlowerAmountDashSpeed").GetComponent<TextMeshProUGUI>();
            dashTimeUpgradeCostText = GameObject.Find("FlowerAmountRoomChance").GetComponent<TextMeshProUGUI>();

            healthNotches = GameObject.Find("HealthLevels").GetComponent<UpgradeNotchUI>();
            speedNotches = GameObject.Find("SpeedLevels").GetComponent<UpgradeNotchUI>();
            currencyNotches = GameObject.Find("FlowerLevels").GetComponent<UpgradeNotchUI>();
            dashTimeNotches = GameObject.Find("DashLevels").GetComponent<UpgradeNotchUI>();
            roomChanceNotches = GameObject.Find("RoomLevels").GetComponent<UpgradeNotchUI>();

            healthUpgradeButton = GameObject.Find("BuyButtonHealth").GetComponent<Button>();
            speedUpgradeButton = GameObject.Find("BuyButtonSpeed").GetComponent<Button>();
            currencyUpgradeButton = GameObject.Find("BuyButtonCurrency").GetComponent<Button>();
            dashTimeUpgradeButton = GameObject.Find("BuyButtonDash").GetComponent<Button>();
            roomChanceUpgradeButton = GameObject.Find("BuyButtonRoomChance").GetComponent<Button>();

            healthUpgradeButton.onClick.RemoveAllListeners();
            healthUpgradeButton.onClick.AddListener(UpgradeHealth);

            speedUpgradeButton.onClick.RemoveAllListeners();
            speedUpgradeButton.onClick.AddListener(UpgradeSpeed);

            currencyUpgradeButton.onClick.RemoveAllListeners();
            currencyUpgradeButton.onClick.AddListener(UpgradeCurrency);

            dashTimeUpgradeButton.onClick.RemoveAllListeners();
            dashTimeUpgradeButton.onClick.AddListener(UpgradeDashTime);

            roomChanceUpgradeButton.onClick.RemoveAllListeners();
            roomChanceUpgradeButton.onClick.AddListener(UpgradeRoomChance);

            SetNotchesAndCostsUI();
            currencyManager.UpdateUI();

            upgradeMenuUI.SetActive(false);

        }

        if(scene.name == "MainGame")
        {
            if (SaveManager.instance)
            {
                SaveManager.instance.LoadPlayerData();
                currencyManager.currentCurrencyAmount = SaveManager.instance.player.currencyAmount;
                currencyManager.UpdateUI();
                healthUpgradeLv = SaveManager.instance.player.healthUpgradeLevel;
                speedUpgradeLv = SaveManager.instance.player.speedUpgradeLevel;
                roomChanceUpgradeLv = SaveManager.instance.player.roomChanceUpgradeLevel;
                currencyUpgradeLv = SaveManager.instance.player.moneyUpgradeLevel;
                dashTimeUpgradeLv = SaveManager.instance.player.dashTimeUpgradeLevel;
                UpdateScriptableObjectStats();
            }

            currencyManager.mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            currencyManager.pauseMenuCurrencyText = GameObject.Find("FlowerAmount").GetComponent<TextMeshProUGUI>();
        }
    }
}
