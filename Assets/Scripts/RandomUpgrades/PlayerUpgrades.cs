using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public RandomUpgrade activeUpgrade;
    public RandomUpgrade passiveUpgrade;

    public bool isActiveRunning = false;

    private PlayerMovement playerMovement;

    [Header("Upgrades")]
    public bool seedBasketActive;
    public float seedBasketTimer;

    public int shieldUses;
    public bool shieldActive;

    public int moneyBoostRoomsRemaining;

    public int dashExplosionUses;
    public bool dashExplosionActive;

    [Header("VFX")]
    public GameObject shieldVFX;

    private void Awake()
    {
        playerMovement = ReferenceManager.Instance.playerMovement;
    }

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
            if (passiveUpgrade != null)
            {
                RemoveUpgrade(passiveUpgrade.behaviour);
            }

            passiveUpgrade = newUpgrade;
            newUpgrade.behaviour.OnAcquire(this);
        }
        else
        {
            if (activeUpgrade != null)
            {
                RemoveUpgrade(activeUpgrade.behaviour);
            }

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
                    if (playerMovement.isRolling)
                    {
                        return;
                    }

                    playerMovement.PlayPlaceAnimation();

                    activeUpgrade = null;
                    isActiveRunning = false;
                }
                break;

            case ActivationType.Toggleable:
                if (isActiveRunning)
                {
                    activeUpgrade.behaviour.OnRemove(this);
                    isActiveRunning = false;

                    if (activeUpgrade.behaviour is SeedBasketUpgrade seedbasket)
                    {
                        playerMovement.StopThrowingSeedsAnimation();
                    }
                }
                else
                {
                    activeUpgrade.behaviour.OnUse(this);
                    isActiveRunning = true;

                    if (activeUpgrade.behaviour is SeedBasketUpgrade seedbasket)
                    {
                        playerMovement.PlayThrowingSeedsAnimation();
                    }
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

        if (activeUpgrade.behaviour is SeedBasketUpgrade seedbasket)
        {
            playerMovement.StopThrowingSeedsAnimation();
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
