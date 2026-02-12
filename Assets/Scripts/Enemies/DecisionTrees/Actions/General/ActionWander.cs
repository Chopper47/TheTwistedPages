using UnityEngine;
using UnityEngine.AI;

public class ActionWander : IAction
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 10.0f;
    [SerializeField] private Transform centerPoint;

    [Header("Movement Settings")]
    [SerializeField] private float minSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 3.5f;

    [Header("Pause Settings")]
    [SerializeField] private float pauseChance = 0.15f;
    [SerializeField] private float minPauseTime = 1.0f;
    [SerializeField] private float maxPauseTime = 3.0f;

    [Header("Repath Settings")]
    [SerializeField] private float repathDelay = 0.5f;

    private NavMeshAgent agent;
    private float repathTimer;
    private bool isPaused;
    private float pauseTimer;

    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();

        if (centerPoint == null)
        {
            centerPoint = transform;
        }

        PickNewDestination(false);
    }

    public override void Act()
    {
        if (!agent || !agent.enabled)
        {
            return;
        }

        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;

            if (pauseTimer <= 0f)
            {
                isPaused = false;
                PickNewDestination(false);
            }

            return;
        }

        repathTimer += Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance < 6.0f && repathTimer >= repathDelay)
        {
            if (Random.value < pauseChance)
            {
                StartPause();
            }
            else
            {
                PickNewDestination(false);
            }

            repathTimer = 0f;
        }

        if (!agent.pathPending && agent.remainingDistance > 0.5f && agent.velocity.sqrMagnitude < 0.1f)
        {
            PickNewDestination(false);
        }

        float distanceFromCenter = Vector3.Distance(agent.transform.position, centerPoint.position);
        if (distanceFromCenter > wanderRadius)
        {
            PickNewDestination(true);
        }
    }

    private void PickNewDestination(bool forceToCenter)
    {
        int maxAttempts = 10;

        float distanceFromCenter = Vector3.Distance(agent.transform.position, centerPoint.position);

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 forwardDirection = agent.velocity.sqrMagnitude > 0.1f ? agent.velocity.normalized : transform.forward;

            Vector3 baseDirection;

            if (forceToCenter || distanceFromCenter > wanderRadius * 1.1f)
            {
                baseDirection = (centerPoint.position - agent.transform.position).normalized;
            }
            else
            {
                Vector3 randomDir = Random.insideUnitSphere;
                randomDir.y = 0f;
                randomDir.Normalize();

                if (Random.value < 0.15f)
                {
                    baseDirection = Vector3.Lerp(forwardDirection, randomDir, 0.8f).normalized;
                }
                else
                {
                    baseDirection = Vector3.Lerp(forwardDirection, randomDir, 0.25f).normalized;
                }

                if (distanceFromCenter > wanderRadius * 0.8f)
                {
                    Vector3 toCenter = (centerPoint.position - agent.transform.position).normalized;
                    baseDirection = Vector3.Lerp(baseDirection, toCenter, 0.4f).normalized;
                }
            }

            Vector3 candidate = agent.transform.position + baseDirection * 8.0f;

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                continue;
            }

            agent.speed = Random.Range(minSpeed, maxSpeed);
            agent.SetDestination(hit.position);

            return;
        }
    }

    private void StartPause()
    {
        isPaused = true;
        pauseTimer = Random.Range(minPauseTime, maxPauseTime);
        agent.ResetPath();
    }
}
