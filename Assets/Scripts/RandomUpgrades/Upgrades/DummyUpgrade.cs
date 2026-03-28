using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Dummy")]
public class DummyUpgrade : PlaceableUpgrade
{
    [SerializeField] private GameObject dummyPrefab;

    public override void OnAcquire(PlayerUpgrades player)
    {

    }

    public override bool OnUse(PlayerUpgrades player)
    {
        bool placed = TryPlace(player, dummyPrefab);

        if (!placed)
        {
            Debug.Log("Cannot place the dummy here.");
        }

        return placed;
    }

    public override void OnRemove(PlayerUpgrades player)
    {

    }
}