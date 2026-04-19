using UnityEngine;
using System.Collections.Generic;

public class DummyTarget : MonoBehaviour
{
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private float affectRadius = 10.0f;

    private Transform playerTransform;
    private List<ChaseAgent> affectedEnemies = new List<ChaseAgent>();

    private void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;

        Collider[] hits = Physics.OverlapSphere(transform.position, affectRadius);
        foreach (Collider hit in hits)
        {
            ChaseAgent agent = hit.GetComponent<ChaseAgent>();
            if (agent != null)
            {
                agent.SetTarget(transform);
                agent.Chase();
                affectedEnemies.Add(agent);
            }
        }

        Invoke(nameof(RemoveDummy), duration);
    }

    private void RemoveDummy()
    {
        foreach (ChaseAgent agent in affectedEnemies)
        {
            if (agent != null)
            {
                agent.SetTarget(playerTransform);
                agent.Chase();
            }
        }
    }
}
