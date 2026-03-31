using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/DashExplosion")]
public class DashExplosionUpgrade : UpgradeBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private int maxUses = 3;
    [SerializeField] private float explosionRadius = 3.0f;
    [SerializeField] private float explosionForce = 5.0f;

    [Header("VFX")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private float particleLifetime = 2.0f;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.dashExplosionUses = maxUses;
        player.dashExplosionActive = false;
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        if (player.dashExplosionActive)
        {
            Deactivate(player);
        }
        else
        {
            Activate(player);
        }

        return true;
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        
    }

    private void Activate(PlayerUpgrades player)
    {
        player.dashExplosionActive = true;
    }

    private void Deactivate(PlayerUpgrades player)
    {
        player.dashExplosionActive = false;
    }

    public void TriggerExplosion(Vector3 position, PlayerUpgrades player)
    {
        if (!player.dashExplosionActive)
        {
            return;
        }

        if (particlePrefab != null)
        {
            GameObject vfx = Instantiate(particlePrefab, position, Quaternion.identity);

            if (particleLifetime > 0)
            {
                Destroy(vfx, particleLifetime);
            }
        }

        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (hit.transform.position - position).normalized;
                    rb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
            }
        }

        player.dashExplosionUses--;

        if (player.dashExplosionUses <= 0)
        {
            Deactivate(player);
            player.RemoveUpgrade(this);
        }
    }
}
