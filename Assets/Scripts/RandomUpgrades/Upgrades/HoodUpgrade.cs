using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Hood")]
public class HoodUpgrade : UpgradeBehaviour
{
    [SerializeField] private int maxRooms = 5;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.hoodRoomsRemaining = maxRooms;
        player.hoodBlockAvailable = true;

        if (player.hoodVisual != null)
        {
            player.hoodVisual.SetActive(true);
        }
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        return false;
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        player.hoodRoomsRemaining = 0;
        player.hoodBlockAvailable = false;

        if (player.hoodVisual != null)
        {
            player.hoodVisual.SetActive(false);
        }
    }
}
