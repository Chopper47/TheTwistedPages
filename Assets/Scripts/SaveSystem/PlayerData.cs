using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currencyAmount;
    public int speedUpgradeLevel;
    public int healthUpgradeLevel;
    public int dashSpeedUpgradeLevel;
    public int moneyUpgradeLevel;
    public int upgradeRoomChanceUpgradeLevel;
    public bool tutorialCompleted;


    public PlayerData(PlayerInfo player)
    {
        currencyAmount = player.currencyAmount;
        speedUpgradeLevel = player.speedUpgradeLevel;
        healthUpgradeLevel = player.healthUpgradeLevel;
        dashSpeedUpgradeLevel = player.dashSpeedUpgradeLevel;
        moneyUpgradeLevel = player.moneyUpgradeLevel;
        upgradeRoomChanceUpgradeLevel = player.upgradeRoomChanceUpgradeLevel;
        tutorialCompleted = player.tutorialCompleted;
    }
}
