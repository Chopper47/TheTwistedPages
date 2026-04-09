using UnityEngine;
using UnityEngine.AI;

public abstract class PlaceableUpgrade : UpgradeBehaviour
{
    [Header("Spawn")]
    [SerializeField] protected float spawnDistance = 1.5f;

    [Header("Validation")]
    [SerializeField] protected float checkRadius = 0.5f;
    [SerializeField] protected LayerMask blockingLayers;

    protected bool TryPlace(PlayerUpgrades player, GameObject prefab)
    {
        Vector3 positionToTry = player.transform.position + player.transform.forward * spawnDistance;

        if (IsOnNavMesh(positionToTry, out NavMeshHit hit))
        {
            Vector3 position = hit.position;
            Quaternion rotation = Quaternion.LookRotation(-player.transform.forward);

            if (!IsOverlapping(position))
            {
                Instantiate(prefab, position, rotation);
                return true;
            }
        }

        return false;
    }

    protected bool IsOnNavMesh(Vector3 position, out NavMeshHit hit)
    {
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private bool IsOverlapping(Vector3 position)
    {
        return Physics.CheckSphere(position, checkRadius, blockingLayers);
    }
}