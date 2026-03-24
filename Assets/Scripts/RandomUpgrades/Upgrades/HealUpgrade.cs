using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Heal")]
public class HealUpgrade : UpgradeBehaviour
{
    [SerializeField] private int healAmount = 2;

    public override void OnAcquire(PlayerUpgrades player)
    {
        
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health == null)
        {
            return false;
        }

        if (health.currentHealth < health.maxHealth)
        {
            health.Heal(healAmount);

            return true;
        }

        return false;
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        
    }
}