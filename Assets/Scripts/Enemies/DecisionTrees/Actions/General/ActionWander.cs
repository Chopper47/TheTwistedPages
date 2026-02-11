using UnityEngine;
using UnityEngine.AI;

public class ActionWander : IAction
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10.0f;
    [SerializeField] private float wanderCooldown = 2.0f;

    private NavMeshAgent agent;
    private float timer;

    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }

    public override void Act()
    {
        if (!agent || !agent.enabled)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= wanderCooldown || agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newPosition = GetRandomPoint(transform.root.position, wanderRadius);

            if (newPosition != Vector3.zero)
            {
                agent.SetDestination(newPosition);
            }

            timer = 0f;
        }
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPosition = center + Random.insideUnitSphere * radius;
            randomPosition.y = center.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}
