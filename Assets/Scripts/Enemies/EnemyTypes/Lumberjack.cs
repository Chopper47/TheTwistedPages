using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
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
    private DecisionTreeAgent decisionTree;
    [SerializeField]private LumberjackAnimations animations;
    [SerializeField] BossAxe bossAxe;
    public List<AudioSource> audioSources;

    [Header("Movement Speeds")]
    [SerializeField] private float pursueSpeed = 3.5f;
    [SerializeField] private float getAxeSpeed = 5.0f;
    [SerializeField] private float enragedSpeed = 6.0f;
    [SerializeField] private float defaultSpeed = 3.5f;

    [Header("Axe State")]
    public bool hasAxe = true;
    public bool receivedAxe = false;
    public bool axeOnGround = false;
    public Vector3 axeGroundPosition;

    [Header("Boss State")]
    public bool isStunned = false;
    public bool isAttacking = false;
    private bool cancelAttack = false;

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

    [SerializeField] private float throwAttackAOffset = 0.5f;
    [SerializeField] private float throwAttackBOffset = 0.5f;

    [Header("Enraged Attack Settings")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private int chargeAmount = 3;
    private int currentChargeCount = 0;
    [SerializeField] private float timeBetweenCharges = 0.5f;
    [Tooltip("Equals to the time it takes to get to one in-game meter")][SerializeField] private float chargeSpeed = 1f;

    [Header("Attacks indicators")]
    [SerializeField] GameObject aoeAIndicator;
    [SerializeField] GameObject aoeBIndicator;
    [SerializeField] GameObject jumpIndicator;

    [SerializeField] float aoeABuildup = 1f;
    [SerializeField] float aoeBBuildup = 1f;
    [SerializeField] float jumpBuildup = 1f;


    string currentAttack;
    private void Awake()
    {
        decisionTree = GetComponent<DecisionTreeAgent>();
        rb = GetComponent<Rigidbody>();
        hb = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        bossAxe.gameObject.SetActive(false);
        bossAxe.SetLumberjack(this);
    }

    public LumberjackAnimations GetAnimations()
    {
        return animations;
    }

    #region Stun
    public void ApplyStun(float duration)
    {
        if (isStunned)
        {
            return;
        }

        Debug.Log("Stunned.");

        animations.StunnedTrigger();

        cancelAttack = true;

        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        CancelCurrentAttackState();

        chaseAgent.StopChase();

        if (agent.enabled)
        {
            agent.isStopped = true;
        }

        enemyState.StayState();

        yield return new WaitForSeconds(duration);

        animations.EndStunnedTrigger();
        decisionTree.SetDecide(true);
        isStunned = false;
        cancelAttack = false;

        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        animations.ResetEndAreaAttack();
        enemyState.StayState();
    }

    private bool ShouldCancelAttack()
    {
        return cancelAttack || isStunned;
    }

    private void CancelCurrentAttackState()
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

        currentChargeCount = 0;
        currentConsecutiveJumps = 0;

        receivedAxe = false;

        SetSpeed(defaultSpeed);

        isAttacking = false;
        enemyState.StayState();
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

    public void PickUpAxe(bool playerThrow, bool truePlayerThrow)
    {
        hasAxe = true;
        axeOnGround = false;
        animations.GetAxe();
        if (!playerThrow)
        {
            animations.StopMovingTrigger();
            if (truePlayerThrow)
            {
                EndAttack();
            }
        }
        if (enemyHealth.health <= 3)
        {
            animations.CloseToAxeTrigger();
        }
    }

    public void ReceiveAxe()
    {
        receivedAxe = true;
        EndAttack();
    }

    public void FalseReceiveAxe()
    {
        receivedAxe = false;
    }
    #endregion

    #region Boss States
    public void Wait()
    {
        SetSpeed(defaultSpeed);
        animations.StopMovingTrigger();
        chaseAgent.StopChase();
    }

    public void GetAxe()
    {
        SetSpeed(getAxeSpeed);
        chaseAgent.Chase();

        if (enemyHealth.health >= 3)
        {
            animations.MovingTrigger();
        }

        chaseAgent.SetNewDestination(axeGroundPosition);
    }

    public void Pursue()
    {
        SetSpeed(pursueSpeed);
        animations.MovingTrigger();
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
        if (isStunned || isAttacking)
        {
            return;
        }
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(EnragedCharge());
    }

    public void DoSwipeAttack()
    {
        if (isStunned || isAttacking)
        {
            return;
        }
        currentAction = StartCoroutine(SwipeAttack());
    }

    public void DoAOEAttackA()
    {
        if (isStunned || isAttacking)
        {
            return;
        }
        currentAction = StartCoroutine(AOEAttackA());
    }

    public void DoAOEAttackB()
    {
        if (isStunned || isAttacking)
        {
            return;
        }
        currentAction = StartCoroutine(AOEAttackB());
    }

    public void DoThrowAttackA()
    {
        if (isStunned || isAttacking)
        {
            return;
        }
        currentAction = StartCoroutine(ThrowAttackA());
    }

    public void DoThrowAttackB()
    {
        if (isStunned || isAttacking)
        {
            return;
        }
        currentAction = StartCoroutine(ThrowAttackB());
    }

    public void DoJumpAttack()
    {
        if (isStunned || isAttacking)
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
        currentAttack = "Swipe";
        yield return MoveSlightlyTowardsPlayer();
        Debug.LogWarning("Doing Swipe Attack");
        audioSources[0].Play();
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
        StartCoroutine(AoeAIndicator());
        currentAttack = "AOE A";
        Debug.LogWarning("Doing AOE Attack A");
        yield return MoveSlightlyTowardsPlayer();
        int groan = UnityEngine.Random.Range(1, 3);
        audioSources[groan].Play();
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
        StartCoroutine(AoeBIndicator());
        currentAttack = "AOE B";
        Debug.LogWarning("Doing AOE Attack B");
        yield return MoveSlightlyTowardsPlayer();
        int groan = UnityEngine.Random.Range(1, 3);
        audioSources[groan].Play();
    }

    public void EnableAoeExpansion()
    {
        StartCoroutine(ExpandAOEHitbox());
        StartCoroutine(EndAOEAttackB());
    }

    private IEnumerator EndAOEAttackB()
    {
        yield return new WaitForSeconds(aoeBExpandTime);
        animations.EndAreaAttackTrigger();
    }

    private IEnumerator ThrowAttackB()
    {
        StartAttack();
        currentAttack = "Throw B";
        Debug.LogWarning("Doing Throw Attack B");
        animations.ResetGetAxe();
        hasAxe = false;
        transform.LookAt(transform.position + chaseAgent.TargetDirection());
        yield return new WaitForSeconds(throwAttackBOffset);
        bossAxe.gameObject.SetActive(true);
        animations.SetLowerHp(enemyHealth.health <= 3 ? true : false);
        animations.EndThrowAttackTrigger();
        bossAxe.StartThrowAttackB(transform.position + Vector3.up*0.5f, chaseAgent.TargetDirection(), axeOnAirTime);
        audioSources[0].Play();
    }

    private IEnumerator ThrowAttackA()
    {
        StartAttack();
        currentAttack = "Throw A";
        Debug.LogWarning("Doing Throw Attack A");
        animations.ResetGetAxe();
        hasAxe = false;
        transform.LookAt(transform.position + chaseAgent.TargetDirection());
        yield return new WaitForSeconds(throwAttackAOffset);
        bossAxe.gameObject.SetActive(true);
        bossAxe.StartThrowAttackA(axeBoomerangTime, chaseAgent.DistanceFromTarget());
        audioSources[0].Play();
        yield return new WaitForSeconds(axeBoomerangTime);
        animations.EndThrowAttackTrigger();
        EndAttack();
    }

    private IEnumerator JumpAttack()
    {
        currentConsecutiveJumps++;

        currentAttack = "Jump";
        Debug.LogWarning("Doing Jump Attack");
        StopCoroutine("ResetJumpCounterAfterDelay");
        StartCoroutine("ResetJumpCounterAfterDelay", 5f);

        StartAttack();
        agent.enabled = false;
        
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        Debug.Log("distance: "+chaseAgent.DistanceFromTarget());

        // Store the player's position at the moment the jump begins (the Boss will always land on this exact point).
        Vector3 targetPosition = chaseAgent.TargetPosition();
        targetPosition.y = transform.position.y;

        Debug.Log("Jump Attack: jumping towards " + targetPosition);
        audioSources[0].Play();

        // Boss "jumps" very high, off-screen, vertically.
        float riseTime = 0.5f;                  // Duration of the upwards movement.
        float timer = 0f;
        //anim
        animations.JumpingTrigger();
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * 25f;

        while (timer < riseTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, peakPosition, timer / riseTime);
            yield return null;
        }
        //anim

        StartCoroutine(JumpIndicator());
        jumpIndicator.transform.localPosition = new Vector3(0, 0.1f, 0);
        animations.FallingTrigger();
        // Horizontal movement towards target in a straight line.
        
        float travelTime = 0.4f;
        timer = 0f;
        Vector3 travelStart = transform.position;
        Vector3 travelEnd = new Vector3(targetPosition.x, travelStart.y, targetPosition.z);

        while (timer < travelTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(travelStart, travelEnd, timer / travelTime);
            jumpIndicator.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            yield return null;
        }

        //anim
        jumpIndicator.transform.parent = null;
        // Boss "falls" straight down towards the target.
        float fallTime = 0.35f;
        timer = 0f;
        Vector3 fallStart = transform.position;
        Vector3 fallEnd = new Vector3(fallStart.x, startPosition.y, fallStart.z);

        while (timer < fallTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(fallStart, fallEnd, timer / fallTime);
            jumpIndicator.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            yield return null;
        }
        transform.position = fallEnd;
        audioSources[4].Play();
        SphereCollider collider = jumpAttackHitbox.GetComponent<SphereCollider>();
        collider.radius = jumpImpactRadius;

        CameraShake.Shake(0.3f, 0.5f);
        jumpAttackHitbox.transform.position = transform.position;
        jumpAttackHitbox.SetActive(true);

        //anim

        yield return new WaitForSeconds(0.5f);

        jumpAttackHitbox.SetActive(false);

        agent.enabled = true;
        animations.StopFalling();
        animations.StopJumpingTrigger();
        animations.ResetStopMovingTrigger();

        EndAttack();
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
        currentAttack = "Enraged";
        audioSources[3].Play();
        SetSpeed(enragedSpeed);

        while (currentChargeCount < chargeAmount)               
        {
            if (ShouldCancelAttack())
            {
                CancelCurrentAttackState();
                yield break;
            }

            float timer = 0f;
            Vector3 dir = chaseAgent.TargetDirection();
            Vector3 startPoint = transform.position;
            Vector3 endPoint = startPoint;
            transform.LookAt(endPoint);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 100f, wallMask))
            {
                endPoint = hit.point - dir * (hb.radius - 0.1f);
            }

            float maxTime = chargeSpeed * Vector3.Distance(startPoint, endPoint);

            EnableHitbox(enragedHitbox);

            while (timer < maxTime)
            {
                if (ShouldCancelAttack())
                {
                    CancelCurrentAttackState();
                    yield break;
                }

                transform.position = Vector3.Lerp(startPoint, endPoint, timer / maxTime);
                timer += Time.deltaTime;
                yield return null;
            }

            DisableHitbox(enragedHitbox);
            currentChargeCount++;
            if(currentChargeCount != chargeAmount)
            {
                animations.EndEnragedTrigger();
                animations.EnragedTrigger();
            }

            yield return new WaitForSeconds(timeBetweenCharges);
        }

        currentChargeCount = 0;
        receivedAxe = false;

        animations.ResetEnragedTrigger();
        animations.EndEnragedTrigger();
        EndAttack();
        SetSpeed(defaultSpeed);
    }
    #endregion

    #region Attack Indicators Coroutines
    
    IEnumerator AoeAIndicator()
    {
        float timer = 0f;
        float maxSize = aoeAHitbox.GetComponent<SphereCollider>().radius * 2;
        aoeAIndicator.transform.localScale = Vector3.zero;
        aoeAIndicator.SetActive(true);
        while (timer < aoeABuildup)
        {
            aoeAIndicator.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxSize, timer / aoeABuildup);
            timer += Time.deltaTime;
            yield return null;
        }
        aoeAIndicator.SetActive(false);
        yield return null;
    }

    IEnumerator AoeBIndicator()
    {
        float timer = 0f;
        float maxSize = aoeBMaxRadius * 2;
        aoeBIndicator.transform.localScale = Vector3.zero;
        aoeBIndicator.SetActive(true);
        while (timer < aoeBBuildup)
        {
            aoeBIndicator.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxSize, timer / aoeBBuildup);
            timer += Time.deltaTime;
            yield return null;
        }
        aoeBIndicator.SetActive(false);
        yield return null;
    }

    IEnumerator JumpIndicator()
    {
        float timer = 0f;
        float maxSize = jumpImpactRadius * 2;
        jumpIndicator.transform.localScale = Vector3.zero;
        jumpIndicator.SetActive(true);
        while (timer < jumpBuildup)
        {
            jumpIndicator.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxSize, timer / jumpBuildup);
            timer += Time.deltaTime;
            yield return null;
        }
        jumpIndicator.SetActive(false);
        yield return null;
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
        if (isStunned || isAttacking)
        {
            return;
        }

        decisionTree.SetDecide(false);
        cancelAttack = false;
        animations.StopMovingTrigger();
        isAttacking = true;
        enemyState.AttackState();
        chaseAgent.StopChase();
    }

    public void StartAttackWOstop()
    {
        if (isStunned || isAttacking)
        {
            return;
        }

        decisionTree.SetDecide(false);
        cancelAttack = false;
        isAttacking = true;
        enemyState.AttackState();
    }

    public void EndAttack()
    {
        decisionTree.SetDecide(true);
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

    /*
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
    */
}
