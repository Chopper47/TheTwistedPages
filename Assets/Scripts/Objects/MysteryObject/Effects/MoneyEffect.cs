using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Money")]
public class MoneyEffect : MysteryEffect
{
    public int amount;

    public override void Apply(GameObject player)
    {
        CurrencyManager.instance.AddCurrency(amount);
    }
}
