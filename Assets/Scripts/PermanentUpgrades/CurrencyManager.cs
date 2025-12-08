using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyManager : MonoBehaviour
{

    public static CurrencyManager instance;

    public int currentCurrencyAmount = 0;

    [SerializeField] private GameObject currencyUI;
    [SerializeField] private TextMeshProUGUI currencyText;

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    public void AddSingleCoin()
    {
        currentCurrencyAmount++;

        StartCoroutine(ShowAndUpdateUI());
    }

    public void AddCurrency(int amount)
    {
        currentCurrencyAmount += amount;
        StartCoroutine(ShowAndUpdateUI());

    }
    public void RemoveCurrency(int amount)
    {
        currentCurrencyAmount -= amount;
        StartCoroutine(ShowAndUpdateUI());
    }

    public bool CheckEnoughCurrencyAmount(int amountToCheck)
    {
        if (currentCurrencyAmount >= amountToCheck) return true;
        return false;
    }

    private IEnumerator ShowAndUpdateUI()
    {
        if (currencyUI == null || currencyText == null) StopCoroutine(ShowAndUpdateUI());

        currencyUI.SetActive(true);
        currencyText.text = currentCurrencyAmount.ToString();

        yield return new WaitForSeconds(2f);

        currencyUI.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currencyUI = SearchCurrencyUI("CurrencyUI");

        if (currencyUI == null) Debug.LogWarning("Could not find CurrencyUI in scene: " + scene.name);

        else
        {
            currencyText = currencyUI.transform.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    private GameObject SearchCurrencyUI(string tag)
    {
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            // Make sure it's a scene object (not a Prefab asset)
            if (obj.CompareTag(tag) && obj.scene.isLoaded)
            {
                return obj;
            }
        }

        return null;
    }
}
