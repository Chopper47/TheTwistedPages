using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RandomUpgrade")]
public class RandomUpgradeEffect : MysteryEffect
{
    public RandomUpgradeList upgradeList;

    public override void Apply(GameObject player)
    {
        PlayerUpgrades playerUpgrades = player.GetComponent<PlayerUpgrades>();

        RandomUpgrade randomUpgrade = upgradeList.GetRandomUpgrade();

        if (playerUpgrades == null || upgradeList == null || randomUpgrade == null)
        {
            return;
        }

        playerUpgrades.AddUpgrade(randomUpgrade);
    }
}
