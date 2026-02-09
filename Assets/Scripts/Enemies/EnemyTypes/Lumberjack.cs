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

    [Header("AOE B Settings")]
    [SerializeField] private float aoeBMaxRadius = 8.0f;
    [SerializeField] private float aoeBExpandTime = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
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
        GetComponent<NavMeshAgent>().SetDestination(axeGroundPosition);
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
        yield return new WaitForSeconds(1.0f);
        hasAxe = true;
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
        yield return new WaitForSeconds(1.5f);
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
    }
}
