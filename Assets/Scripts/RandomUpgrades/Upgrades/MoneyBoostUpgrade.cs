using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/MoneyBoost")]
public class MoneyBoostUpgrade : UpgradeBehaviour
{
    [SerializeField] private float moneyMultiplier = 0.5f;

    public int durationInRooms = 3;

    private int roomsRemaining;

    public override void OnAcquire(PlayerUpgrades player)
    {
        roomsRemaining = durationInRooms;

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
        roomsRemaining--;

        if (roomsRemaining <= 0)
        {
            player.passiveUpgrade = null;
        }
    }
}
