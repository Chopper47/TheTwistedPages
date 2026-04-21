using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    public int currentCurrencyAmount = 0;
    [SerializeField] private UpgradeStats upgradeStatsScriptableObject;
    [SerializeField] private float moneyMultiplier = 1.0f;

    [Header("UI References")]
    public TextMeshProUGUI pauseMenuCurrencyText;
    public TextMeshProUGUI upgradeMenuCurrencyText;

    [Header("Popup Settings")]
    [SerializeField] private CurrencyPopup currencyPopupPrefab;
    public Canvas mainCanvas;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);


    }

    private void Start()
    {
        if (SaveManager.instance)
        {
            SaveManager.instance.LoadPlayerData();
            currentCurrencyAmount = SaveManager.instance.player.currencyAmount;
        }
        AddMoneyMultiplier(upgradeStatsScriptableObject.currency + 1);
        UpdateUI();
    }

    public void AddSingleCoin()
    {
        currentCurrencyAmount++;
    }

    public void AddCurrency(int amount)
    {
        int finalAmount = Mathf.RoundToInt(amount * moneyMultiplier);
        currentCurrencyAmount += finalAmount;
        if (SaveManager.instance)
        {
            SaveManager.instance.player.currencyAmount = currentCurrencyAmount;
            SaveManager.instance.SavePlayerData();
        }

        ShowPopup(finalAmount);
        UpdateUI();
    }

    public void RemoveCurrency(int amount)
    {
        currentCurrencyAmount -= amount;

        if (currentCurrencyAmount < 0)
        {
            currentCurrencyAmount = 0;
        }

        if (SaveManager.instance)
        {
            SaveManager.instance.player.currencyAmount = currentCurrencyAmount;
            SaveManager.instance.SavePlayerData();
        }

        UpdateUI();
    }

    public void AddMoneyMultiplier(float multiplier)
    {
        moneyMultiplier *= multiplier;
    }

    public void RemoveMoneyMultiplier(float multiplier)
    {
        moneyMultiplier /= multiplier;
    }

    public bool CheckEnoughCurrencyAmount(int amountToCheck)
    {
        return currentCurrencyAmount >= amountToCheck;
    }

    private void ShowPopup(int amount)
    {
        if (currencyPopupPrefab == null || mainCanvas == null)
        {
            return;
        }

        CurrencyPopup popup = Instantiate(currencyPopupPrefab, mainCanvas.transform);

        popup.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        popup.Setup(amount);
    }

    public void UpdateUI()
    {
        if (pauseMenuCurrencyText != null)
        {
            pauseMenuCurrencyText.text = currentCurrencyAmount.ToString();
        }
        if(upgradeMenuCurrencyText != null)
        {
            upgradeMenuCurrencyText.text = currentCurrencyAmount.ToString();
        }
    }
}