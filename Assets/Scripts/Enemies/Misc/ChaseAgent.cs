using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAgent : MonoBehaviour
{
    public float speed = 1f;
    public Transform target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LumberjackAnimations animations;
    [SerializeField] bool usingNavMesh = true;

    [Header("Follow Path variables (Only if not using nav mesh)")]
    [SerializeField] bool followPath = false;
    [SerializeField] float pointTimeInterval = 0.2f;
    bool chase = false;
    bool startChase = false;
    bool following = true; 
    Vector3 chaseDir;

    Rigidbody rb;
    EnemyHealth enemyHealth;
    EnemyState states;

    /// <summary>
    /// Sets the speed of the enemy in the NavMesh
    /// </summary>
    void Start()
    {
        if (target == null)
        {
            target = FindFirstObjectByType<PlayerMovement>().transform;
        }
        agent.speed = speed;
        enemyHealth = GetComponent<EnemyHealth>();
        states = GetComponent<EnemyState>();
        rb = GetComponent<Rigidbody>();

        if(!usingNavMesh)
        {
            chaseDir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;
        }
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

    private void Update()
    {

        if(!usingNavMesh && target != null && chase)
        {
            if(!followPath)
            {
                if(startChase)
                {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                    UpdateChaseDirection();
                    transform.Translate(chaseDir * speed * Time.deltaTime, Space.World);
                }
            }
            else
            {
                if (following)
                {
                    Debug.Log("followpath");
                    following = false;
                    FollowPath(pointTimeInterval);
                }
            }
        }
    }

    /// <summary>
    /// Updates the direction in which the enemy follows the player
    /// </summary>
    public void UpdateChaseDirection()
    {
        chaseDir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;
    }

    public void FollowPath(float timeToPoint)
    {
        StartCoroutine(GetToPoint(timeToPoint));
    }
    IEnumerator GetToPoint(float maxTime)
    {
        if (chase)
        {
            float timer = 0f;
            Vector3 startingPos = transform.position;
            Vector3 endingPos = target.position;
            while (timer < maxTime)
            {
                transform.position = Vector3.Lerp(startingPos, endingPos, timer/maxTime);
                timer += Time.deltaTime * speed;
                yield return null;
            }
            StartCoroutine(GetToPoint(maxTime));
        }
        else
        {
            yield return null;
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

    public bool IsMoving()
    {
        return Mathf.Approximately(agent.velocity.magnitude, 0f);
    }

    /// <summary>
    /// Method for knowing what distance is the enemy from the player
    /// </summary>
    /// <returns>Returns the distance between the player and the enemy in a float</returns>
    public float DistanceFromTarget()
    {
        if(target != null) return Vector3.Distance(transform.position, target.position);
        return -99f;
    }

    /// <summary>
    /// Method for knowing the player position
    /// </summary>
    /// <returns>Returns the position of the player</returns>
    public Vector3 TargetPosition()
    {
        if(target != null) return target.position;
        return transform.position;
    }

    /// <summary>
    /// Method for changing chase speed of nav mesh agent
    /// </summary>
    /// <param name="newSpeed">New wanted speed</param>
    public void SetVelocity(float newSpeed)
    {
        agent.speed = newSpeed;
    }

    /// <summary>
    /// Method for making the enemy stay in its place
    /// </summary>
    public void Stay()
    {
        chase = false;
        if (followPath)
        {
            following = true;
        }

        if (usingNavMesh)
        {
            states.StayState();
            agent.SetDestination(transform.position);
        }
    }

    /// <summary>
    /// Method for resuming the chase
    /// </summary>
    public void Chase()
    {
        states.ChaseState();
        if(rb)
        {
            rb.linearVelocity = Vector3.zero;
        }
        chase = true;

        if(usingNavMesh)
        {
            agent.SetDestination(target.transform.position);
            agent.isStopped = false;
        }
    }

    public void BossChase()
    {
        Chase();
        animations.Moving();
    }

    /// <summary>
    /// Method for getting normalized direction between current gameobject and its target
    /// </summary>
    /// <returns>Normalized Vector</returns>
    public Vector3 TargetDirection()
    {
        return new Vector3(target.transform.position.x - transform.position.x, 0f, target.transform.position.z - transform.position.z).normalized;
    }

    public void SetNewDestination(Vector3 newDestination)
    {
        agent.SetDestination(newDestination);
    }

    /// <summary>
    /// Method for stopping the chase
    /// </summary>
    public void StopChase()
    {
        Stay();
        chase = false;

        if(usingNavMesh)
        {
           agent.isStopped = true;
        }
    }

    public void LookAtTarget()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetStartChase(bool newStartChase)
    {
        startChase = newStartChase;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(transform.position, transform.position + chaseDir * 2f);
    }
}
