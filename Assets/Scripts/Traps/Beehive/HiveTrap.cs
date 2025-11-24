using UnityEngine;

public class HiveTrap : Trap
{
    [SerializeField] private GameObject beeSwarmPrefab;
    [SerializeField] private float swarmDuration = 5.0f;

    private bool triggered = false;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (triggered)
        {
            return;
        }

        GameObject swarm = Instantiate(beeSwarmPrefab, transform.position, Quaternion.identity);

        BeeSwarm beeSwarm = swarm.GetComponent<BeeSwarm>();
        beeSwarm.Initialize(other.transform, swarmDuration);

        triggered = true;
    }
}
