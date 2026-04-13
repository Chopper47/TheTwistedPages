using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomUpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public RandomUpgrade newUpgrade;
    [SerializeField] private GameObject menu;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    [SerializeField] private Image newUpgradeIcon;
    [SerializeField] private TextMeshProUGUI newUpgradeDescription;
    [SerializeField] private Image currentUpgradeIcon;
    [SerializeField] private TextMeshProUGUI currentUpgradeDescription;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu = transform.GetChild(0).gameObject;
        playerUpgrades = FindFirstObjectByType<PlayerUpgrades>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNewUpgrade(RandomUpgrade upgradeToAdd)
    {
        newUpgrade = upgradeToAdd;
        //newUpgradeIcon.sprite = upgradeToAdd.icon;
        //newUpgradeDescription.text = upgradeToAdd.description;

        if(newUpgrade.type == UpgradeType.Passive)
        {
            //currentUpgradeIco n.sprite = playerUpgrades.passiveUpgrade.icon;
            //currentUpgradeDescription.text = playerUpgrades.passiveUpgrade.description;
        }
        else if (newUpgrade.type == UpgradeType.Active)
        {
            //currentUpgradeIcon.sprite = playerUpgrades.activeUpgrade.icon;
            //currentUpgradeDescription.text = upgradeToAdd.acctiveUpgrade.description;
        }
    }
    public void AddUpgrade()
    {
        playerUpgrades.AddUpgrade(newUpgrade);
        CloseMenu();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }
    public void CloseMenu()
    {   
        menu.SetActive(false);
    }
}
