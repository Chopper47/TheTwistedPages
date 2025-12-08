using UnityEngine;

public class CurrencyManager : MonoBehaviour
{

    public static CurrencyManager instance;

    public int currentCurrencyAmount = 0;

    private void Awake()
    {
        instance = this;
    }
    public void AddSingleCoin()
    {
        currentCurrencyAmount++;
    }

    public void AddCurrency(int amount)
    {
        currentCurrencyAmount += amount;
    }
    public void RemoveCurrency(int amount)
    {
        currentCurrencyAmount -= amount;
    }

    public bool CheckEnoughCurrencyAmount(int amountToCheck)
    {
        if (currentCurrencyAmount >= amountToCheck) return true;
        return false;
    }
}
