using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAgent : MonoBehaviour
{
    public float speed = 1f;
    Transform target;
    [SerializeField] NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if(movement)
        {
            target = other.transform;
        }
    }

    public bool TargetExist()
    {
        if(target)
        {
            return true;
        }

        return false;
    }

    public float DistanceFromTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    public Vector3 TargetPosition()
    {
        return target.position;
    }

    public void Stay()
    {
        agent.SetDestination(transform.position);
    }

    public void Chase()
    {
        agent.SetDestination(target.transform.position);
        agent.isStopped = false;
    }

    public void StopChase()
    {
        Stay();
        agent.isStopped = true;
    }
}
