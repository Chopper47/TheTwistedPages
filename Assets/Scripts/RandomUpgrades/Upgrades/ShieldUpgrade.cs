using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Shield")]
public class ShieldUpgrade : UpgradeBehaviour
{
    [SerializeField] private int maxUses = 3;

    private int currentUses = 0;
    private bool active = false;

    public override void OnAcquire(PlayerUpgrades playerUpgrades)
    {

    }

    public override bool OnUse(PlayerUpgrades playerUpgrades)
    {
        if (active)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }

        return true;
    }

    public override void OnRemove(PlayerUpgrades playerUpgrades)
    {
        if (active)
        {
            Deactivate();
        }
    }

    
    private void Activate()
    {
        currentUses = maxUses;
        active = true;

        PlayerMovement.isInvincible = true;
        Debug.Log("Shield activated.");
    }

    private void Deactivate()
    {
        active = false;

        PlayerMovement.isInvincible = false;
        Debug.Log("Shield deactivated.");
    }

    public bool TryBlockDamage(PlayerUpgrades player)
    {
        if (!active)
        {
            return false;
        }

        currentUses--;

        if (currentUses <= 0)
        {
            Deactivate();

            if (player != null)
            {
                player.activeUpgrade = null;
                player.isActiveRunning = false;
            }
        }

        return true;
    }

    public bool IsActive()
    {
        return active;
    }

    public int RemainingUses()
    {
        return currentUses;
    }
}
