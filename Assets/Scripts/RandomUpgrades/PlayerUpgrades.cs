using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public RandomUpgrade activeUpgrade;
    public RandomUpgrade passiveUpgrade;

    private bool isActiveRunning = false;

    public void AddUpgrade(RandomUpgrade newUpgrade)
    {
        if (newUpgrade == null)
        {
            return;
        }

        if (newUpgrade.type == UpgradeType.Passive)
        {
            passiveUpgrade = newUpgrade;
            newUpgrade.behaviour.OnAcquire(this);
        }
        else
        {
            activeUpgrade = newUpgrade;

            if (newUpgrade.activationType == ActivationType.Automatic)
            {
                newUpgrade.behaviour.OnUse(this);
            }
        }
    }

    public void UseActiveUpgrade()
    {
        if (activeUpgrade == null)
        {
            return;
        }

        switch (activeUpgrade.activationType)
        {
            case ActivationType.OneUse:
                activeUpgrade.behaviour.OnUse(this);
                activeUpgrade = null;
                break;

            case ActivationType.Toggleable:
                if (isActiveRunning)
                {
                    activeUpgrade.behaviour.OnRemove(this);
                    isActiveRunning = false;
                }
                else
                {
                    activeUpgrade.behaviour.OnUse(this);
                    isActiveRunning = true;
                }
                break;
        }
    }
}
