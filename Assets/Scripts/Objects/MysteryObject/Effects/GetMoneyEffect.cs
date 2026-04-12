using UnityEngine;

[CreateAssetMenu(menuName = "Effects/GetMoney")]
public class GetMoneyEffect : MysteryEffect
{
    public int amount;

    public override void Apply(GameObject player)
    {
        CurrencyManager.instance.AddCurrency(amount);
    }
}
