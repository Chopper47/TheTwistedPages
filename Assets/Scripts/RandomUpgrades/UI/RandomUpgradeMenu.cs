using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomUpgradeMenu : MonoBehaviour
{
    [Header("References")]
    public RandomUpgrade newUpgrade;
    private PlayerUpgrades playerUpgrades;

    [Header("Background")]
    [SerializeField] private Sprite activeBackground;     
    [SerializeField] private Sprite passiveBackground;

    [Header("New Upgrade")]
    [SerializeField] private Image newUpgradeBackground;
    [SerializeField] private Image newUpgradeIcon;
    [SerializeField] private TextMeshProUGUI newUpgradeTitle;
    [SerializeField] private TextMeshProUGUI newUpgradeType;
    [SerializeField] private TextMeshProUGUI newUpgradeDescription;

    [Header("Current Upgrade")]
    [SerializeField] private GameObject currentUpgradeContainer;

    [SerializeField] private Image currentUpgradeBackground;
    [SerializeField] private Image currentUpgradeIcon;
    [SerializeField] private TextMeshProUGUI currentUpgradeTitle;
    [SerializeField] private TextMeshProUGUI currentUpgradeType;
    [SerializeField] private TextMeshProUGUI currentUpgradeDescription;
    
    private void Awake()
    {
        playerUpgrades = FindFirstObjectByType<PlayerUpgrades>();
    }

    public void SetNewUpgrade(RandomUpgrade upgradeToAdd)
    {
        newUpgrade = upgradeToAdd;

        newUpgradeIcon.sprite = upgradeToAdd.icon;
        newUpgradeTitle.text = upgradeToAdd.displayName;
        newUpgradeType.text = upgradeToAdd.typeAndCharges;
        newUpgradeDescription.text = upgradeToAdd.description;

        RandomUpgrade current = null;

        if (newUpgrade.type == UpgradeType.Passive)
        {
            current = playerUpgrades.passiveUpgrade;

            newUpgradeBackground.sprite = passiveBackground;
        }
        else
        {
            current = playerUpgrades.activeUpgrade;

            newUpgradeBackground.sprite = activeBackground;
        }

        if (current != null)
        {
            currentUpgradeContainer.SetActive(true);

            currentUpgradeIcon.sprite = current.icon;
            currentUpgradeTitle.text = current.displayName;
            currentUpgradeType.text = current.typeAndCharges;
            currentUpgradeDescription.text = current.description;

            if (current.type == UpgradeType.Passive)
            {
                currentUpgradeBackground.sprite = passiveBackground;
            }
            else
            {
                currentUpgradeBackground.sprite = activeBackground;
            }
        }
        else
        {
            currentUpgradeContainer.SetActive(false);
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
