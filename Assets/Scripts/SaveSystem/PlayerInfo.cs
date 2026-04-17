using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(SaveManager))]
public class PlayerInfo : MonoBehaviour
{
    [Header("Player Stats")]
    public int currencyAmount;
    public int speedUpgradeLevel;
    public int healthUpgradeLevel;
    public int roomChanceUpgradeLevel;
    public int dashTimeUpgradeLevel;
    public int moneyUpgradeLevel;

    [Header("Player Progress")]
    public bool tutorialCompleted;

    

    

}
