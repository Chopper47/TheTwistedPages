using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currencyAmount;
    public int speedUpgradeLevel;
    public int healthUpgradeLevel;
    public int dashTimeUpgradeLevel;
    public int moneyUpgradeLevel;
    public int roomChanceUpgradeLevel;
    public bool tutorialCompleted;


    public PlayerData(PlayerInfo player)
    {
        currencyAmount = player.currencyAmount;
        speedUpgradeLevel = player.speedUpgradeLevel;
        healthUpgradeLevel = player.healthUpgradeLevel;
        dashTimeUpgradeLevel = player.dashTimeUpgradeLevel;
        moneyUpgradeLevel = player.moneyUpgradeLevel;
        roomChanceUpgradeLevel = player.roomChanceUpgradeLevel;
        tutorialCompleted = player.tutorialCompleted;
    }
}
