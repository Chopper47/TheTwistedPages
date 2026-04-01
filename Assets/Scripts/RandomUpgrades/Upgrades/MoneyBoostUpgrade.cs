using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/MoneyBoost")]
public class MoneyBoostUpgrade : UpgradeBehaviour
{
    [SerializeField] private float moneyMultiplier = 2.0f;

    public int durationInRooms = 3;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.moneyBoostRoomsRemaining = durationInRooms;

        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.AddMoneyMultiplier(moneyMultiplier);
        }
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        return false;
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        if (CurrencyManager.instance != null)
        {
            CurrencyManager.instance.RemoveMoneyMultiplier(moneyMultiplier);
        }
    }

    public void OnRoomCleared(PlayerUpgrades player)
    {
        player.moneyBoostRoomsRemaining--;

        if (player.moneyBoostRoomsRemaining <= 0)
        {
            player.RemoveUpgrade(this);
        }
    }
}
