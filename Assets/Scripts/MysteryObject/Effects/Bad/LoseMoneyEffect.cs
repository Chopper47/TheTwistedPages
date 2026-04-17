using UnityEngine;

[CreateAssetMenu(menuName = "Effects/LoseMoney")]
public class LoseMoneyEffect : MysteryEffect
{
    public int amount;

    public override void Apply(GameObject player)
    {
        CurrencyManager.instance.RemoveCurrency(amount);
    }
}
