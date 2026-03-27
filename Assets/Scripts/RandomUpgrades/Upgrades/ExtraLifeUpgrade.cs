using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/ExtraLife")]
public class ExtraLifeUpgrade : UpgradeBehaviour
{
    [SerializeField] private int extraLives = 2;

    public override void OnAcquire(PlayerUpgrades player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.AddExtraLife(extraLives);
        }
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        return false;
    }

    public override void OnRemove(PlayerUpgrades player)
    {

    }
}