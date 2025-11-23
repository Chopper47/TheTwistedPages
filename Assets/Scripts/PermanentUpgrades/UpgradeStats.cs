using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrades")]
public class UpgradeStats : ScriptableObject
{
    public int health;
    public int speed;
    public int bulletTime;
    public int PushPower;
}
