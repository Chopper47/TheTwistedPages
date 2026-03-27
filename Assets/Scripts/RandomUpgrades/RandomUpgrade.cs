using UnityEngine;

public enum UpgradeID
{
    SeedBasket,
    Shield,
    MoneyBoost,
    DashExplosion,
    Heal,
    ExtraLife,
    Dummy,
    Reroll,
    BearTrap
}

public enum UpgradeType
{
    Active,
    Passive
}

public enum ActivationType
{
    Automatic,
    OneUse,
    Toggleable
}

[CreateAssetMenu(menuName = "ScriptableObjects/RandomUpgrades/RandomUpgrade")]
public class RandomUpgrade : ScriptableObject
{
    public UpgradeID id;
    public UpgradeType type;
    public ActivationType activationType;

    public float duration;

    public UpgradeBehaviour behaviour;
}
