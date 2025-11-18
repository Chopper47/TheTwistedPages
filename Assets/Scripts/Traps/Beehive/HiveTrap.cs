using UnityEngine;

public class HiveTrap : MonoBehaviour
{
    [SerializeField] private GameObject beeSwarmPrefab;
    [SerializeField] private float swarmDuration = 4f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameObject swarm = Instantiate(beeSwarmPrefab, transform.position, Quaternion.identity);

            BeeSwarm beeSwarm = swarm.GetComponent<BeeSwarm>();
            beeSwarm.Initialize(other.transform, swarmDuration);

            triggered = true;
        }
    }
}
