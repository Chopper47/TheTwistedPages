using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RandomUpgrade")]
public class RandomUpgradeEffect : MysteryEffect
{
    public RandomUpgradeList upgradeList;

    public override void Apply(GameObject player)
    {
        RandomUpgradeMenu upgradeMenu = FindFirstObjectByType<RandomUpgradeMenu>();

        if (upgradeMenu == null || upgradeList == null)
        {
            return;
        }

        RandomUpgrade randomUpgrade = upgradeList.GetRandomUpgrade();

        if (randomUpgrade == null)
        {
            return;
        }

        upgradeMenu.SetNewUpgrade(randomUpgrade);
        upgradeMenu.OpenMenu();
    }
}