using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Shield")]
public class ShieldUpgrade : UpgradeBehaviour
{
    [SerializeField] private int maxUses = 3;

    public override void OnAcquire(PlayerUpgrades playerUpgrades)
    {
        playerUpgrades.shieldUses = maxUses;
        playerUpgrades.shieldActive = false;
    }

    public override bool OnUse(PlayerUpgrades playerUpgrades)
    {
        if (playerUpgrades.shieldActive)
        {
            Deactivate(playerUpgrades);
        }
        else
        {
            Activate(playerUpgrades);
        }

        return true;
    }

    public override void OnRemove(PlayerUpgrades playerUpgrades)
    {
        if (playerUpgrades.shieldActive)
        {
            Deactivate(playerUpgrades);
        }
    }

    private void Activate(PlayerUpgrades player)
    {
        player.shieldActive = true;
        PlayerMovement.isInvincible = true;
    }

    private void Deactivate(PlayerUpgrades player)
    {
        player.shieldActive = false;
        PlayerMovement.isInvincible = false;
    }

    public bool TryBlockDamage(PlayerUpgrades player)
    {
        if (!player.shieldActive)
        {
            return false;
        }

        player.shieldUses--;

        Debug.Log("Shield uses: " + player.shieldUses);

        if (player.shieldUses <= 0)
        {
            Deactivate(player);
            player.RemoveUpgrade(this);
        }

        return true;
    }
}
