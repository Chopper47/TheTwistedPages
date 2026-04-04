using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public RandomUpgrade activeUpgrade;
    public RandomUpgrade passiveUpgrade;

    public bool isActiveRunning = false;

    [Header("Upgrades")]
    public bool seedBasketActive;
    public float seedBasketTimer;

    public int shieldUses;
    public bool shieldActive;

    public int moneyBoostRoomsRemaining;

    public int dashExplosionUses;
    public bool dashExplosionActive;

    private void Update()
    {
        if (activeUpgrade != null)
        {
            if (activeUpgrade.behaviour is SeedBasketUpgrade seedBasket)
            {
                seedBasket.Tick(this, transform, GetComponent<PlayerMovement>().GetDirection());
            }
        }
    }

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

            newUpgrade.behaviour.OnAcquire(this);

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
                bool usedSuccessfully = activeUpgrade.behaviour.OnUse(this);

                if (usedSuccessfully)
                {
                    activeUpgrade = null;
                    isActiveRunning = false;
                }
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

    public void RemoveUpgrade(UpgradeBehaviour behaviour)
    {
        if (behaviour == null)
        {
            return;
        }

        behaviour.OnRemove(this);

        if (activeUpgrade != null && activeUpgrade.behaviour == behaviour)
        {
            activeUpgrade = null;
            isActiveRunning = false;
        }

        if (passiveUpgrade != null && passiveUpgrade.behaviour == behaviour)
        {
            passiveUpgrade = null;
        }
    }
}
