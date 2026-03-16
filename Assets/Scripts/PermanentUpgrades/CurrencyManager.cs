using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    public int currentCurrencyAmount = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI pauseMenuCurrencyText;

    [Header("Popup Settings")]
    [SerializeField] private CurrencyPopup currencyPopupPrefab;
    [SerializeField] private Canvas mainCanvas;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
        PlayerPrefs.DeleteKey("CompletedTutorial");
#endif

    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddSingleCoin()
    {
        currentCurrencyAmount++;
    }

    public void AddCurrency(int amount)
    {
        currentCurrencyAmount += amount;
        ShowPopup(amount);
        UpdateUI();
    }

    public void RemoveCurrency(int amount)
    {
        currentCurrencyAmount -= amount;
        UpdateUI();
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

    private void UpdateUI()
    {
        if (pauseMenuCurrencyText != null)
        {
            pauseMenuCurrencyText.text = currentCurrencyAmount.ToString();
        }
    }
}