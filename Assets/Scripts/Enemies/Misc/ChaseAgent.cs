using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAgent : MonoBehaviour
{
    public float speed = 1f;
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;

    Rigidbody rb;
    EnemyHealth enemyHealth;
    EnemyState states;

    private void Awake()
    {
        if(target == null)
        {
           target = FindFirstObjectByType<PlayerMovement>().transform;
        }
    }

    /// <summary>
    /// Sets the speed of the enemy in the NavMesh
    /// </summary>
    void Start()
    {
        agent.speed = speed;
        enemyHealth = GetComponent<EnemyHealth>();
        states = GetComponent<EnemyState>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// If there's a player in the trigger zone the variable target is assigned to the player. TODO remove and changed not using trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if(movement)
        {
            target = other.transform;
        }
    }

    /// <summary>
    /// Method for checking if the target is null or not
    /// </summary>
    /// <returns>If target is null returns false, if target isn't null it returns true</returns>
    public bool TargetExist()
    {
        if(target)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method for knowing what distance is the enemy from the player
    /// </summary>
    /// <returns>Returns the distance between the player and the enemy in a float</returns>
    public float DistanceFromTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    /// <summary>
    /// Method for knowing the player position
    /// </summary>
    /// <returns>Returns the position of the player</returns>
    public Vector3 TargetPosition()
    {
        return target.position;
    }

    /// <summary>
    /// Method for making the enemy stay in its place
    /// </summary>
    public void Stay()
    {
        states.StayState();
        agent.SetDestination(transform.position);
    }

    /// <summary>
    /// Method for resuming the chase
    /// </summary>
    public void Chase()
    {
        states.ChaseState();
        rb.linearVelocity = Vector3.zero;
        agent.SetDestination(target.transform.position);
        agent.isStopped = false;
    }

    /// <summary>
    /// Method for stopping the chase
    /// </summary>
    public void StopChase()
    {
        Stay();
        agent.isStopped = true;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
