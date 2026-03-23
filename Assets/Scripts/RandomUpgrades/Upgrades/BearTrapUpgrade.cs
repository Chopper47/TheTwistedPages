using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Upgrades/BearTrap")]
public class BearTrapUpgrade : UpgradeBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private float spawnDistance = 1.5f;

    [Header("Validation")]
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask blockingLayers;

    public override void OnAcquire(PlayerUpgrades player)
    {

    }

    public override bool OnUse(PlayerUpgrades player)
    {
        Vector3 spawnPosition = player.transform.position + player.transform.forward * spawnDistance;

        if (IsValidPosition(spawnPosition))
        {
            Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
            return true;
        }
        else
        {
            Debug.Log("Cannot place the bear trap here.");
            return false;
        }
    }

    public override void OnRemove(PlayerUpgrades player) 
    {

    }

    private bool IsValidPosition(Vector3 position)
    {
        return IsOnNavMesh(position) && !IsOverlapping(position);
    }

    private bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private bool IsOverlapping(Vector3 position)
    {
        return Physics.CheckSphere(position, checkRadius, blockingLayers);
    }
}