using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Lumberjack : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private CapsuleCollider hb;
    private NavMeshAgent agent;
    private EnemyState enemyState;
    private ChaseAgent chaseAgent;
    private EnemyHealth enemyHealth;
    [SerializeField]private LumberjackAnimations animations;
    [SerializeField] BossAxe bossAxe;

    [Header("Movement Speeds")]
    [SerializeField] private float pursueSpeed = 3.5f;
    [SerializeField] private float getAxeSpeed = 5.0f;
    [SerializeField] private float enragedSpeed = 6.0f;
    [SerializeField] private float defaultSpeed = 3.5f;

    [Header("Axe State")]
    public bool hasAxe = true;
    public bool recievedAxe = false;
    public bool axeOnGround = false;
    public Vector3 axeGroundPosition;

    [Header("Boss State")]
    public bool isStunned = false;
    public bool isAttacking = false;

    private Coroutine currentAction;

    [Header("Distances")]
    [SerializeField] private float closeRange = 4.0f;
    [SerializeField] private float midRange = 10.0f;

    [Header("Pre-Attack Movement")]
    [SerializeField] private float preAttackMoveTime = 1.0f;
    [SerializeField] private float preAttackSpeed = 4.5f;
    [SerializeField] private float preAttackMinDistance = 2.5f;

    [Header("Hitboxes")]
    [SerializeField] private GameObject aoeAHitbox;
    [SerializeField] private GameObject jumpAttackHitbox;
    [SerializeField] private GameObject enragedHitbox;
    [SerializeField] private GameObject swipeHitbox;

    [Header("AOE Settings")]
    [SerializeField] private float aoeBMaxRadius = 8.0f;
    [SerializeField] private float aoeBExpandTime = 1.0f;
    [SerializeField] private Transform aoeBRoot;
    [SerializeField] private SphereCollider aoeBSphereHitbox;

    private List<SphereCollider> activeSpheres = new List<SphereCollider>();

    [Header("Jump Attack Settings")]
    [SerializeField] private float jumpImpactRadius = 5.0f;
    [SerializeField] private int maxConsecutiveJumps = 3;
    private int currentConsecutiveJumps = 0;

    [Header("Throw Attacks Settings")]
    [SerializeField] private float axeOnAirTime = .1f;
    [SerializeField] private float axeBoomerangTime = 1.5f; 

    [Header("Enraged Attack Settings")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private int chargeAmount = 3;
    private int currentChargeCount = 0;
    [SerializeField] private float timeBetweenCharges = 0.5f;
    [Tooltip("Equals to the time it takes to get to one in-game meter")][SerializeField] private float chargeSpeed = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hb = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        bossAxe.gameObject.SetActive(false);
        bossAxe.SetLumberjack(this);
    }

    #region Stun
    public void ApplyStun(float duration)
    {
        Debug.Log("Stunned.");

        if (isStunned)
        {
            return;
        }

        currentConsecutiveJumps = 0;
        currentChargeCount = 0;

        if (currentAction != null)
        {
            StopCoroutine(currentAction);
        }

        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        isAttacking = false;

        ForceStopAllAttacks();

        chaseAgent.StopChase();

        if (agent.enabled)
        {
            agent.isStopped = true;
        }

        enemyState.StayState();

        yield return new WaitForSeconds(duration);

        isStunned = false;

        if (agent.enabled)
        {
            agent.isStopped = false;
        }

        enemyState.StayState();
    }

    private void ForceStopAllAttacks()
    {
        DisableHitbox(aoeAHitbox);
        DisableHitbox(jumpAttackHitbox);
        DisableHitbox(enragedHitbox);
        DisableHitbox(swipeHitbox);

        foreach (SphereCollider sphere in activeSpheres)
        {
            if (sphere)
            {
                Destroy(sphere.gameObject);
            }
        }

        activeSpheres.Clear();

        rb.isKinematic = false;

        if (!agent.enabled)
        {
            agent.enabled = true;
        }
    }
    #endregion

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

    public void PickUpAxe()
    {
        hasAxe = true;
        axeOnGround = false;
        animations.GainAxe();
        if(enemyHealth.health <= 3)
        {
            animations.CloseToAxe();
        }
    }

    public void RecieveAxe()
    {
        recievedAxe = true;
        EndAttack();
    }

    public void FalseRecieveAxe()
    {
        recievedAxe = false;
    }
    #endregion

    #region Boss States
    public void Wait()
    {
        SetSpeed(defaultSpeed);
        animations.StopMoving();
        chaseAgent.StopChase();
    }

    public void GetAxe()
    {
        SetSpeed(getAxeSpeed);
        chaseAgent.Chase();

        if (enemyHealth.health >= 3)
        {
            animations.Moving();
        }

        chaseAgent.SetNewDestination(axeGroundPosition);
    }

    public void Pursue()
    {
        SetSpeed(pursueSpeed);
        animations.Moving();
        chaseAgent.Chase();
    }

    public void CanGetAxe()
    {
        axeOnGround = true;
        axeGroundPosition = bossAxe.transform.position;
        EndAttack();
    }

    public bool CanDoJumpAttack()
    {
        return currentConsecutiveJumps < maxConsecutiveJumps && !isStunned;
    }
    #endregion

    #region Attacks
    public void DoEnragedAttack()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(EnragedCharge());
    }

    public void DoSwipeAttack()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(SwipeAttack());
    }

    public void DoAOEAttackA()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(AOEAttackA());
    }

    public void DoAOEAttackB()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(AOEAttackB());
    }

    public void DoThrowAttackA()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(ThrowAttackA());
    }

    public void DoThrowAttackB()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(ThrowAttackB());
    }

    public void DoJumpAttack()
    {
        if (isStunned)
        {
            return;
        }

        currentAction = StartCoroutine(JumpAttack());
    }
    #endregion

    #region Attack Coroutines
    private IEnumerator SwipeAttack()
    {
        StartAttack();
        yield return MoveSlightlyTowardsPlayer();
        animations.StartSwipeAttack();
    }

    public void EnableSwipeHitbox()
    {
        EnableHitbox(swipeHitbox);
    }

    public void DisableSwipeHitbox()
    {
        DisableHitbox(swipeHitbox);
    }

    private IEnumerator AOEAttackA()
    {
        StartAttack();
        yield return MoveSlightlyTowardsPlayer();
        animations.StartAreaAttackA();
    }

    public void EnableAoeHitbox()
    {
        EnableHitbox(aoeAHitbox);
    }

    public void DisableAoeHitbox()
    {
        DisableHitbox(aoeAHitbox);
    }

    private IEnumerator AOEAttackB()
    {
        StartAttack();
        yield return MoveSlightlyTowardsPlayer();
        animations.StartAreaAttackB();
    }

    public void EnableAoeExpansion()
    {
        StartCoroutine(ExpandAOEHitbox());
        StartCoroutine(EndAOEAttackB());
    }

    private IEnumerator EndAOEAttackB()
    {
        yield return new WaitForSeconds(aoeBExpandTime);
        animations.EndAreaAttack();
    }

    private IEnumerator ThrowAttackB()
    {
        StartAttack();
        animations.StartThrowAttackB();
        hasAxe = false;
        transform.LookAt(transform.position + chaseAgent.TargetDirection());
        yield return new WaitForSeconds(0.2f);
        bossAxe.gameObject.SetActive(true);
        animations.SetLowerHp(enemyHealth.health <= 3 ? true : false);
        animations.EndThrowAttack();
        bossAxe.StartThrowAttackB(transform.position + Vector3.up*0.5f, chaseAgent.TargetDirection(), axeOnAirTime);
    }

    private IEnumerator ThrowAttackA()
    {
        StartAttack();
        animations.StartThrowAttackA();
        hasAxe = false;
        transform.LookAt(transform.position + chaseAgent.TargetDirection());
        yield return new WaitForSeconds(0.2f);
        bossAxe.gameObject.SetActive(true);
        bossAxe.StartThrowAttackA(axeBoomerangTime, chaseAgent.DistanceFromTarget());
        yield return new WaitForSeconds(axeBoomerangTime);
        animations.EndThrowAttack();
        EndAttack();
    }

    private IEnumerator JumpAttack()
    {
        currentConsecutiveJumps++;

        StopCoroutine("ResetJumpCounterAfterDelay");
        StartCoroutine("ResetJumpCounterAfterDelay", 5f);

        StartAttack();

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
        //anim
        animations.Jumping();
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * 25f;

        while (timer < riseTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, peakPosition, timer / riseTime);
            yield return null;
        }
        //anim
        animations.StopJumping();
        animations.Falling();
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

        //anim
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

        animations.StopFalling();

        //anim

        yield return new WaitForSeconds(0.5f);

        jumpAttackHitbox.SetActive(false);

        agent.enabled = true;

        EndAttack();

        animations.StopMoving();
    }

    private IEnumerator ResetJumpCounterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentConsecutiveJumps = 0;
        Debug.Log("Jump counter reset after " + delay + " seconds");
    }

    private IEnumerator EnragedCharge()
    {
        StartAttack();
        float timer;
        float maxTime;
        Vector3 dir, endPoint, startPoint;
        endPoint = Vector3.zero;
        Debug.Log("Enraged start");

        SetSpeed(enragedSpeed);

        while (currentChargeCount < chargeAmount)
        {
            Debug.Log("loop start");
            timer = 0f;
            dir = chaseAgent.TargetDirection();
            startPoint = transform.position;
            Debug.Log("raycast start");
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 100f, wallMask))
            {
                endPoint = hit.point - dir * (hb.radius - 0.1f);
            }

            maxTime = chargeSpeed * Vector3.Distance(startPoint, endPoint);
            EnableHitbox(enragedHitbox);
            Debug.Log("Should charge in " + maxTime + " seconds");
            while (timer < maxTime)
            {
                transform.position = Vector3.Lerp(startPoint, endPoint, timer / maxTime);
                timer += Time.deltaTime;
                yield return null;
            }
            Debug.Log("End charge");

            currentChargeCount++;
            DisableHitbox(enragedHitbox);
            yield return new WaitForSeconds(timeBetweenCharges);
        }

        currentChargeCount = 0;

        recievedAxe = false;
        EndAttack();

        SetSpeed(defaultSpeed);
    }
    #endregion

    private void SetSpeed(float newSpeed)
    {
        chaseAgent.SetVelocity(newSpeed);
    }

    private IEnumerator MoveSlightlyTowardsPlayer()
    {
        float timer = 0f;

        SetSpeed(preAttackSpeed);
        agent.isStopped = false;

        while (timer < preAttackMoveTime)
        {
            float distance = chaseAgent.DistanceFromTarget();

            if (distance <= preAttackMinDistance)
            {
                break;
            }

            agent.SetDestination(chaseAgent.TargetPosition());

            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = true;
    }

    private void StartAttack()
    {
        if (isStunned)
        {
            return;
        }
        animations.StopMoving();
        isAttacking = true;
        enemyState.AttackState();
        chaseAgent.StopChase();
    }

    public void EndAttack()
    {
        isAttacking = false;
        currentAction = null;
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
        float timer = 0f;

        float sphereRadius = aoeBSphereHitbox.radius;
        float maxRingRadius = aoeBMaxRadius;

        int sphereCount = Mathf.CeilToInt((Mathf.PI * maxRingRadius) / sphereRadius);

        for (int i = 0; i < sphereCount; i++)
        {
            SphereCollider newSphere = Instantiate(aoeBSphereHitbox, aoeBRoot);
            newSphere.gameObject.SetActive(true);
            activeSpheres.Add(newSphere);
        }

        while (timer < aoeBExpandTime)
        {
            timer += Time.deltaTime;
            float t = timer / aoeBExpandTime;
            float currentRadius = Mathf.Lerp(0f, maxRingRadius, t);

            for (int i = 0; i < activeSpheres.Count; i++)
            {
                float angle = (Mathf.PI * 2f / sphereCount) * i;

                Vector3 offset = new Vector3(Mathf.Cos(angle) * currentRadius, 0f, Mathf.Sin(angle) * currentRadius);

                activeSpheres[i].transform.localPosition = offset;
            }

            yield return null;
        }

        foreach (SphereCollider sphere in activeSpheres)
        {
            Destroy(sphere.gameObject);
        }

        activeSpheres.Clear();
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

        if (aoeBSphereHitbox)
        {
            SphereCollider collider = aoeBSphereHitbox.GetComponent<SphereCollider>();
            if (collider)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(aoeBSphereHitbox.transform.position, collider.radius);
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
