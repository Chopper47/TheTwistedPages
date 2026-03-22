using UnityEngine;

public enum UpgradeID
{
    SeedBasket,
    Shield,
    MoneyBoost,
    DashExplosion,
    Heal,
    Dummy,
    Reroll,
    BearTrap
}

public enum UpgradeType
{
    Active,
    Passive
}

[CreateAssetMenu(menuName = "ScriptableObjects/RandomUpgrades/RandomUpgrade")]
public class RandomUpgrade : ScriptableObject
{
    public UpgradeID id;
    public UpgradeType type;

    public float duration;

    public GameObject pickupPrefab;
}
