using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/BearTrap")]
public class BearTrapUpgrade : PlaceableUpgrade
{
    [SerializeField] private GameObject bearTrapPrefab;

    public override void OnAcquire(PlayerUpgrades player)
    {

    }

    public override bool OnUse(PlayerUpgrades player)
    {
        bool placed = TryPlace(player, bearTrapPrefab);

        if (!placed)
        {
            Debug.Log("Cannot place the bear trap here.");
        }

        return placed;
    }

    public override void OnRemove(PlayerUpgrades player)
    {

    }
}