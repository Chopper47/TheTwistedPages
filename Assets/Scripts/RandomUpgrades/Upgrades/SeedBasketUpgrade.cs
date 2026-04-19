using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/SeedBasket")]
public class SeedBasketUpgrade : UpgradeBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration = 10.0f;
    [SerializeField] private float dropInterval = 0.5f;
    [SerializeField] private GameObject rootPrefab;
    [SerializeField] private float spawnOffset = 0.7f;
    private float dropTimer;

    public override void OnAcquire(PlayerUpgrades player)
    {
        player.seedBasketActive = false;
        player.seedBasketTimer = 0f;
    }

    public override bool OnUse(PlayerUpgrades player)
    {
        if (player.seedBasketActive)
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
        Deactivate(player);
    }

    private void Activate(PlayerUpgrades player)
    {
        player.seedBasketActive = true;
        player.seedBasketTimer = duration;
        dropTimer = 0f;
    }

    private void Deactivate(PlayerUpgrades player)
    {
        player.seedBasketActive = false;
    }

    public void Tick(PlayerUpgrades player, Transform playerTransform, Vector3 moveDirection)
    {
        if (!player.seedBasketActive)
        {
            return;
        }

        player.seedBasketTimer -= Time.deltaTime;

        if (player.seedBasketTimer <= 0f)
        {
            player.RemoveUpgrade(this);
            return;
        }

        if (moveDirection.sqrMagnitude < 0.1f)
        {
            return;
        }

        dropTimer -= Time.deltaTime;

        if (dropTimer <= 0f)
        {
            SpawnRoot(playerTransform, moveDirection);

            player.AM.Play("Root", player.Main);

            dropTimer = dropInterval;
        }
    }

    private void SpawnRoot(Transform playerTransform, Vector3 moveDirection)
    {
        if (rootPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = playerTransform.position - moveDirection.normalized * spawnOffset;

        RoomController room = CameraController.Instance.currentRoom;

        if (room != null)
        {
            Instantiate(rootPrefab, spawnPosition, Quaternion.identity, room.transform);
        }
    }

    public override string GetCounterText(PlayerUpgrades player)
    {
        return player.seedBasketTimer.ToString();
    }
}
