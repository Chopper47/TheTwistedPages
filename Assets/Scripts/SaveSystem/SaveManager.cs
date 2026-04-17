using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public PlayerInfo player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        LoadPlayerData();
    }
    public void SavePlayerData()
    {
        SaveSystem.SaveData(player);
    }

    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        Debug.Log("LOADING");

        player.currencyAmount = data.currencyAmount;
        player.speedUpgradeLevel = data.speedUpgradeLevel;
        player.healthUpgradeLevel = data.healthUpgradeLevel;
        player.dashTimeUpgradeLevel = data.dashTimeUpgradeLevel;
        player.moneyUpgradeLevel = data.moneyUpgradeLevel;
        player.roomChanceUpgradeLevel = data.roomChanceUpgradeLevel;
        player.tutorialCompleted = data.tutorialCompleted;

    }

    
}
