using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Shield")]
public class ShieldUpgrade : UpgradeBehaviour
{
    [SerializeField] private int maxUses = 3;

    public override void OnAcquire(PlayerUpgrades playerUpgrades)
    {
        playerUpgrades.shieldCharges = maxUses;
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

        if (player.shieldVFX != null)
        {
            player.shieldVFX.SetActive(true);
        }
    }

    private void Deactivate(PlayerUpgrades player)
    {
        player.shieldActive = false;
        PlayerMovement.isInvincible = false;

        if (player.shieldVFX != null)
        {
            player.shieldVFX.SetActive(false);
        }
    }

    public bool TryBlockDamage(PlayerUpgrades player)
    {
        if (!player.shieldActive)
        {
            return false;
        }

        player.shieldCharges--;

        Debug.Log("Shield uses: " + player.shieldCharges);

        if (player.shieldCharges <= 0)
        {
            Deactivate(player);
            player.RemoveUpgrade(this);
        }

        return true;
    }
}
