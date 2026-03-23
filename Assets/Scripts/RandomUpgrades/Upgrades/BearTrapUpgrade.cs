using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/BearTrap")]
public class BearTrapUpgrade : UpgradeBehaviour
{
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private float spawnDistance = 1.5f;

    public override void OnAcquire(PlayerUpgrades player)
    {
        
    }

    public override void OnUse(PlayerUpgrades player)
    {
        Vector3 spawnPosition = player.transform.position + player.transform.forward * spawnDistance;

        Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
    }

    public override void OnRemove(PlayerUpgrades player)
    {
        
    }
}