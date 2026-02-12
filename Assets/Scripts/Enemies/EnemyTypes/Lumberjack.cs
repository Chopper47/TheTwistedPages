using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Lumberjack : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private EnemyState enemyState;
    private ChaseAgent chaseAgent;
    private EnemyHealth enemyHealth;
    [SerializeField] BossAxe bossAxe;

    [Header("Axe State")]
    public bool hasAxe = true;
    public bool axeReturning = false;
    public bool axeOnGround = false;
    public Vector3 axeGroundPosition;

    [Header("Boss State")]
    public bool isStunned = false;
    public bool isAttacking = false;

    [Header("Distances")]
    [SerializeField] private float closeRange = 4.0f;
    [SerializeField] private float midRange = 10.0f;

    [Header("Hitboxes")]
    [SerializeField] private GameObject aoeAHitbox;
    [SerializeField] private GameObject aoeBHitbox;
    [SerializeField] private GameObject jumpAttackHitbox;

    [Header("AOE Settings")]
    [SerializeField] private float aoeBMaxRadius = 8.0f;
    [SerializeField] private float aoeBExpandTime = 1.0f;

    [Header("Jump Attack Settings")]
    [SerializeField] private float jumpRiseHeight = 4.0f;
    [SerializeField] private float jumpAirTime = 1.0f;
    [SerializeField] private float jumpFallTime = 0.4f;
    [SerializeField] private float jumpImpactRadius = 5.0f;

    [Header("Throw Attacks Settings")]
    [SerializeField] private float axeOnAirTime = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        bossAxe.gameObject.SetActive(false);
    }

    #region Distance Checks
    public bool TargetInCloseRange()
    {
        return chaseAgent.DistanceFromTarget() <= closeRange;
    }

    public bool TargetInMidRange()
    {
        float distance = chaseAgent.DistanceFromTarget();
        return distance > closeRange && distance <= midRange;
    }

    public bool TargetInLongRange()
    {
        return chaseAgent.DistanceFromTarget() > midRange;
    }
    #endregion

    #region Axe States
    public void SetAxeReturningState(bool value)
    {
        axeReturning = value;
        if (value)
        {
            axeOnGround = false;
            hasAxe = false;
        }
    }

    public void SetAxeOnGround(Vector3 position)
    {
        axeOnGround = true;
        axeGroundPosition = position;
        axeReturning = false;
        hasAxe = false;
    }

    public void PickUpAxe()
    {
        hasAxe = true;
        axeReturning = false;
        axeOnGround = false;
    }
    #endregion

    #region Boss States
    public void Wait()
    {
        chaseAgent.StopChase();
    }

    public void ReceiveAxe()
    {
        PickUpAxe();
    }

    public void GetAxe()
    {
        chaseAgent.Chase();
        chaseAgent.SetNewDestination(axeGroundPosition);
    }

    public void Pursue()
    {
        chaseAgent.Chase();
    }
    #endregion

    #region Attacks
    public void DoSwipeAttack()
    {
        StartCoroutine(SwipeAttack());
    }

    public void DoAOEAttackA()
    {
        StartCoroutine(AOEAttackA());
    }

    public void DoAOEAttackB()
    {
        StartCoroutine(AOEAttackB());
    }

    public void DoThrowAttackA()
    {
        StartCoroutine(ThrowAttackA());
    }

    public void DoThrowAttackB()
    {
        StartCoroutine(ThrowAttackB());
    }

    public void DoJumpAttack()
    {
        StartCoroutine(JumpAttack());
    }
    #endregion

    #region Attack Coroutines
    private IEnumerator SwipeAttack()
    {
        StartAttack();
        yield return new WaitForSeconds(0.8f);
        EndAttack();
    }

    private IEnumerator AOEAttackA()
    {
        StartAttack();
        EnableHitbox(aoeAHitbox);
        yield return new WaitForSeconds(0.5f);
        DisableHitbox(aoeAHitbox);
        EndAttack();
    }

    private IEnumerator AOEAttackB()
    {
        StartAttack();
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(ExpandAOEHitbox());
        yield return new WaitForSeconds(0.4f);
        EndAttack();
    }

    private IEnumerator ThrowAttackA()
    {
        StartAttack();
        hasAxe = false;
        yield return new WaitForSeconds(0.2f);
        bossAxe.gameObject.SetActive(true);
        bossAxe.StartThrowAttackA(transform.position + Vector3.up*0.1f, new Vector3(chaseAgent.TargetPosition().x - transform.position.x,0f, chaseAgent.TargetPosition().z - transform.position.z).normalized, axeOnAirTime);
        yield return new WaitForSeconds(axeOnAirTime);
        axeOnGround = true;
        axeGroundPosition = bossAxe.transform.position;
        EndAttack();
    }

    private IEnumerator ThrowAttackB()
    {
        StartAttack();
        hasAxe = false;
        yield return new WaitForSeconds(1.4f);
        hasAxe = true;
        EndAttack();
    }

    private IEnumerator JumpAttack()
    {
        StartAttack();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        // Store the player's position at the moment the jump begins (the Boss will always land on this exact point).
        Vector3 targetPosition = chaseAgent.TargetPosition();
        targetPosition.y = transform.position.y;

        Debug.Log("Jump Attack: jumping towards " + targetPosition);

        // Boss "jumps" very high, off-screen, vertically.
        float riseTime = 0.5f;                  // Duration of the upwards movement.
        float timer = 0f;
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * 25f;

        while (timer < riseTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, peakPosition, timer / riseTime);
            yield return null;
        }

        // Horizontal movement towards target in a straight line.
        float travelTime = 0.4f;
        timer = 0f;
        Vector3 travelStart = transform.position;
        Vector3 travelEnd = new Vector3(targetPosition.x, travelStart.y, targetPosition.z);

        while (timer < travelTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(travelStart, travelEnd, timer / travelTime);
            yield return null;
        }

        // Boss "falls" straight down towards the target.
        float fallTime = 0.35f;
        timer = 0f;
        Vector3 fallStart = transform.position;
        Vector3 fallEnd = new Vector3(fallStart.x, startPosition.y, fallStart.z);

        while (timer < fallTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(fallStart, fallEnd, timer / fallTime);
            yield return null;
        }

        SphereCollider collider = jumpAttackHitbox.GetComponent<SphereCollider>();
        collider.radius = jumpImpactRadius;

        jumpAttackHitbox.transform.position = transform.position;
        jumpAttackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        jumpAttackHitbox.SetActive(false);

        agent.enabled = true;

        EndAttack();
    }
    #endregion

    private void StartAttack()
    {
        isAttacking = true;
        enemyState.AttackState();
        chaseAgent.StopChase();
    }

    private void EndAttack()
    {
        isAttacking = false;
        enemyState.StayState();
    }

    public void EnableHitbox(GameObject hitbox)
    {
        hitbox.SetActive(true);
    }

    public void DisableHitbox(GameObject hitbox)
    {
        hitbox.SetActive(false);
    }

    private IEnumerator ExpandAOEHitbox()
    {
        SphereCollider collider = aoeBHitbox.GetComponent<SphereCollider>();
        collider.radius = 0f;

        aoeBHitbox.SetActive(true);

        float timer = 0f;

        while (timer < aoeBExpandTime)
        {
            timer += Time.deltaTime;
            float t = timer / aoeBExpandTime;

            collider.radius = Mathf.Lerp(0f, aoeBMaxRadius, t);

            yield return null;
        }

        aoeBHitbox.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (aoeAHitbox)
        {
            SphereCollider collider = aoeAHitbox.GetComponent<SphereCollider>();
            if (collider)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(aoeAHitbox.transform.position, collider.radius);
            }
        }

        if (aoeBHitbox)
        {
            SphereCollider collider = aoeBHitbox.GetComponent<SphereCollider>();
            if (collider)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(aoeBHitbox.transform.position, collider.radius);
            }
        }

        if (jumpAttackHitbox)
        {
            SphereCollider collider = jumpAttackHitbox.GetComponent<SphereCollider>();
            if (collider)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(jumpAttackHitbox.transform.position, jumpImpactRadius);
            }
        }
    }
}
