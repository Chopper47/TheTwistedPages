using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currencyAmount;
    public int speedUpgradeLevel;
    public int healthUpgradeLevel;
    public bool tutorialCompleted;


    public PlayerData(PlayerInfo player)
    {
        currencyAmount = player.currencyAmount;
        speedUpgradeLevel = player.speedUpgradeLevel;
        healthUpgradeLevel = player.healthUpgradeLevel;
        tutorialCompleted = player.tutorialCompleted;
    }
}
