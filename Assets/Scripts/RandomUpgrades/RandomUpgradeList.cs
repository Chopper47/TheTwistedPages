using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RandomUpgrades/RandomUpgradeList")]
public class RandomUpgradeList : ScriptableObject
{
    public List<RandomUpgrade> upgrades;

    public RandomUpgrade GetRandomUpgrade()
    {
        if (upgrades == null || upgrades.Count == 0)
        {
            return null;
        }

        return upgrades[Random.Range(0, upgrades.Count)];
    }
}
