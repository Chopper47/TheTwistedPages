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

    public int shieldCharges;
    public bool shieldActive;

    public int moneyBoostRoomsRemaining;

    public int dashExplosionCharges;
    public bool dashExplosionActive;

    public int hoodRoomsRemaining;
    public bool hoodBlockAvailable;

    public int magicTrailCharges;
    public bool magicTrailActive;

    [Header("VFX")]
    public GameObject shieldVFX;

    public GameObject hoodVisualEquipped;
    public GameObject hoodVisualUnequipped;

    [Header("SFX")]
    public AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        playerMovement = ReferenceManager.Instance.playerMovement;
        AM = FindAnyObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        if (activeUpgrade != null)
        {
            if (activeUpgrade.behaviour is SeedBasketUpgrade seedBasket)
            {
                seedBasket.Tick(this, transform, GetComponent<PlayerMovement>().GetDirection());
            }

            if (activeUpgrade.behaviour is MagicTrailUpgrade magicTrail)
            {
                magicTrail.Tick(this, transform, GetComponent<PlayerMovement>().GetDirection());
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
                    isActiveRunning = false;
                    activeUpgrade.behaviour.OnUse(this);

                    if (activeUpgrade.behaviour is SeedBasketUpgrade seedbasket)
                    {
                        playerMovement.StopThrowingSeedsAnimation();
                    }
                }
                else
                {
                    isActiveRunning = true;
                    activeUpgrade.behaviour.OnUse(this);

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

    public bool TryBlockDamage()
    {
        if (activeUpgrade != null && activeUpgrade.behaviour is ShieldUpgrade shield)
        {
            if (shield.TryBlockDamage(this))
            {
                return true;
            }
        }

        if (passiveUpgrade != null && passiveUpgrade.behaviour is HoodUpgrade)
        {
            if (hoodBlockAvailable)
            {
                hoodBlockAvailable = false;

                if (hoodVisualEquipped != null)
                {
                    hoodVisualEquipped.SetActive(false);
                }

                if (hoodVisualUnequipped != null)
                {
                    hoodVisualUnequipped.SetActive(true);
                }

                return true;
            }
        }

        return false;
    }

    public void OnEnterNormalRoom()
    {
        if (hoodRoomsRemaining <= 0)
        {
            return;
        }

        hoodBlockAvailable = true;
        hoodRoomsRemaining--;

        if (hoodVisualEquipped != null)
        {
            hoodVisualEquipped.SetActive(true);
        }

        if (hoodVisualUnequipped != null)
        {
            hoodVisualUnequipped.SetActive(true);
        }

        if (hoodRoomsRemaining <= 0)
        {
            if (passiveUpgrade != null && passiveUpgrade.behaviour is HoodUpgrade)
            {
                RemoveUpgrade(passiveUpgrade.behaviour);
            }
        }
    }
}
