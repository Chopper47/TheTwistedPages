using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHUDManager : MonoBehaviour
{
    private PlayerUpgrades playerUpgrades;

    [Header("Backgrounds")]
    [SerializeField] private Image activeBackground;
    [SerializeField] private Image passiveBackground;

    [Header("Icons")]
    [SerializeField] private Image activeIcon;
    [SerializeField] private Image passiveIcon;
    
    [Header("Counters")]
    [SerializeField] private TextMeshProUGUI activeCounterText;
    [SerializeField] private TextMeshProUGUI passiveCounterText;

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

            string counter = playerUpgrades.activeUpgrade.behaviour.GetCounterText(playerUpgrades);

            if (!string.IsNullOrEmpty(counter))
            {
                activeCounterText.gameObject.SetActive(true);
                activeCounterText.text = counter;
            }
            else
            {
                activeCounterText.gameObject.SetActive(false);
            }

            if (playerUpgrades.activeUpgrade.activationType == ActivationType.Toggleable)
            {
                if (playerUpgrades.isActiveRunning)
                {
                    activeIcon.color = normalColor;
                    activeBackground.color = normalColor;
                }
                else
                {
                    activeIcon.color = disabledColor;
                    activeBackground.color = disabledColor;
                }
            }
            else
            {
                activeIcon.color = normalColor;
                activeBackground.color = normalColor;
            }
        }
        else
        {
            activeIcon.enabled = false;
            activeCounterText.gameObject.SetActive(false);
        }

        if (playerUpgrades.passiveUpgrade != null)
        {
            passiveIcon.enabled = true;
            passiveIcon.sprite = playerUpgrades.passiveUpgrade.icon;

            string counter = playerUpgrades.passiveUpgrade.behaviour.GetCounterText(playerUpgrades);

            if (!string.IsNullOrEmpty(counter))
            {
                passiveCounterText.gameObject.SetActive(true);
                passiveCounterText.text = counter;
            }
            else
            {
                passiveCounterText.gameObject.SetActive(false);
            }
        }
        else
        {
            passiveIcon.enabled = false;
            passiveCounterText.gameObject.SetActive(false);
        }
    }
}
