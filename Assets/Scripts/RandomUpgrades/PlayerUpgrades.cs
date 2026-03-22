using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public RandomUpgrade activeUpgrade;
    public RandomUpgrade passiveUpgrade;

    public void AddUpgrade(RandomUpgrade newUpgrade)
    {
        if (newUpgrade == null)
        {
            return;
        }

        switch (newUpgrade.type)
        {
            case UpgradeType.Active:
                // Replace current active type upgrade.
                if (activeUpgrade != null)
                {
                    Debug.Log("Replacing current active upgrade: " + activeUpgrade.id);
                }
                activeUpgrade = newUpgrade;
                Debug.Log("New active upgrade: " + activeUpgrade.id);
                break;

            case UpgradeType.Passive:
                // Replace current passive type upgrade.
                if (passiveUpgrade != null)
                {
                    Debug.Log("Replacing current passive upgrade: " + passiveUpgrade.id);
                    // aquí puedes desactivar efectos de la antigua pasiva si hace falta
                }
                passiveUpgrade = newUpgrade;
                Debug.Log("New passive upgrade: " + passiveUpgrade.id);
                break;
        }

        ApplyUpgrade(newUpgrade);
    }

    // Aplica efectos de la mejora al jugador
    private void ApplyUpgrade(RandomUpgrade upgrade)
    {
        // Temporary structure.
        switch (upgrade.id)
        {
            case UpgradeID.SeedBasket:
                // TODO.
                break;

            case UpgradeID.Shield:
                // TODO.
                break;

            case UpgradeID.MoneyBoost:
                // TODO.
                break;

            // case...

            default:
                break;
        }
    }
}