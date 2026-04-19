using UnityEngine;
using UnityEngine.UI;

public class UpgradeHUDManager : MonoBehaviour
{
    private PlayerUpgrades playerUpgrades;

    [Header("Icons")]
    [SerializeField] private Image activeIcon;
    [SerializeField] private Image passiveIcon;

    [Header("Colours")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = new Color(0.4f, 0.4f, 0.4f);

    private void Awake()
    {
        if (playerUpgrades == null)
        {
            playerUpgrades = FindFirstObjectByType<PlayerUpgrades>();
        }
    }

    private void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (playerUpgrades.activeUpgrade != null)
        {
            activeIcon.enabled = true;
            activeIcon.sprite = playerUpgrades.activeUpgrade.icon;

            if (playerUpgrades.activeUpgrade.activationType == ActivationType.Toggleable)
            {
                if (playerUpgrades.isActiveRunning)
                {
                    activeIcon.color = normalColor;
                }
                else
                {
                    activeIcon.color = disabledColor;
                }
            }
            else
            {
                activeIcon.color = normalColor;
            }
        }
        else
        {
            activeIcon.enabled = false;
        }

        if (playerUpgrades.passiveUpgrade != null)
        {
            passiveIcon.enabled = true;
            passiveIcon.sprite = playerUpgrades.passiveUpgrade.icon;
        }
        else
        {
            passiveIcon.enabled = false;
        }
    }
}
