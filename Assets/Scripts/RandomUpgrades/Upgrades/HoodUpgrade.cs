using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Hood")]
public class HoodUpgrade : UpgradeBehaviour
{
    [SerializeField] private int maxRooms = 5;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.hoodRoomsRemaining = maxRooms;
        player.hoodBlockAvailable = true;

        if (player.hoodVisualEquipped != null)
        {
            player.hoodVisualEquipped.SetActive(true);
        }

        if (player.hoodVisualUnequipped != null)
        {
            player.hoodVisualUnequipped.SetActive(false);
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

        if (player.hoodVisualEquipped != null)
        {
            player.hoodVisualEquipped.SetActive(false);
            player.hoodVisualUnequipped.SetActive(false);
        }
    }
}
