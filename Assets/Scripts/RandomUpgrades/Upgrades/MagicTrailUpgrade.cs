using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/MagicTrail")]
public class MagicTrailUpgrade : UpgradeBehaviour
{
    [Header("Settings")]
    public GameObject trailVFX;
    public float spawnInterval = 0.1f;
    public float radius = 1.5f;
    public int damage = 1;
    public int maxCharges = 5;

    private float timer;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.magicTrailCharges = maxCharges;
        player.magicTrailActive = false;
        timer = 0f;
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        player.magicTrailActive = !player.magicTrailActive;
        return true;
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        player.magicTrailActive = false;
    }

    public void Tick(PlayerUpgrades player, Transform playerTransform, Vector3 moveDir)
    {
        if (!player.magicTrailActive)
            return;

        if (player.magicTrailCharges <= 0)
        {
            player.magicTrailActive = false;
            player.RemoveUpgrade(this);
            return;
        }

        PlayerMovement playerMovement = ReferenceManager.Instance.playerMovement;
        if (!playerMovement.isRolling)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Spawn(playerTransform);
            timer = spawnInterval;
        }
    }

    private void Spawn(Transform playerTransform)
    {
        Vector3 pos = playerTransform.position;

        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hit, 2f))
        {
            pos = hit.point;
        }

        GameObject vfx = Instantiate(trailVFX, pos, Quaternion.identity);
    }
}
