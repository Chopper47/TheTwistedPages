using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomUpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public RandomUpgrade newUpgrade;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    [SerializeField] private Image newUpgradeIcon;
    [SerializeField] private TextMeshProUGUI newUpgradeDescription;
    [SerializeField] private Image currentUpgradeIcon;
    [SerializeField] private TextMeshProUGUI currentUpgradeDescription;
    
    private void Awake()
    {
        playerUpgrades = FindFirstObjectByType<PlayerUpgrades>();
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

    public void TakeUpgrade()
    {
        playerUpgrades.AddUpgrade(newUpgrade);
        CloseMenu();
    }

    public void LeaveUpgrade()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        UIManager.Instance.OpenBlockingMenu(UIManager.Instance.randomUpgradeMenu);
    }

    public void CloseMenu()
    {
        UIManager.Instance.CloseBlockingMenu(UIManager.Instance.randomUpgradeMenu);
    }
}
