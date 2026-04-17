using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrades")]
public class UpgradeStats : ScriptableObject
{
    public int health;
    public int speed;
    public int roomChance;
    public int currency;
    public int dashTime;
}
