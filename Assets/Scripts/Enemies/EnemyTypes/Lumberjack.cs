using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lumberjack : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private CapsuleCollider hb;
    private EnemyState enemyState;
    private ChaseAgent chaseAgent;
    private EnemyHealth enemyHealth;
    [SerializeField] BossAxe bossAxe;

    [Header("Axe State")]
    public bool hasAxe = true;
    public bool recievedAxe = false;
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
    [SerializeField] private GameObject jumpAttackHitbox;
    [SerializeField] private GameObject enragedHitbox;

    [Header("AOE Settings")]
    [SerializeField] private float aoeBMaxRadius = 8.0f;
    [SerializeField] private float aoeBExpandTime = 1.0f; 
    [SerializeField] private Transform aoeBRoot;
    [SerializeField] private SphereCollider aoeBSphereHitbox;

    [SerializeField] private float expandTime = 1.0f;

    private List<SphereCollider> activeSpheres = new List<SphereCollider>();

    [Header("Jump Attack Settings")]
    [SerializeField] private float jumpRiseHeight = 4.0f;
    [SerializeField] private float jumpAirTime = 1.0f;
    [SerializeField] private float jumpFallTime = 0.4f;
    [SerializeField] private float jumpImpactRadius = 5.0f;

    [Header("Throw Attacks Settings")]
    [SerializeField] private float axeOnAirTime = 1.5f;

    [Header("Enraged Attack Settings")]

    [SerializeField] private LayerMask wallMask;
    [SerializeField] private int chargeAmount = 3;
    [SerializeField] private float timeBetweenCharges = 0.5f;
    [Tooltip("Equals to the time it takes to get to one in-game meter")][SerializeField] private float chargeSpeed = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hb = GetComponent<CapsuleCollider>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        bossAxe.gameObject.SetActive(false);
        bossAxe.SetLumberjack(this);
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

    public void PickUpAxe()
    {
        hasAxe = true;
        axeOnGround = false;
    }

    public void RecieveAxe()
    {
        recievedAxe = true;
        EndAttack();
    }
    #endregion

    #region Boss States
    public void Wait()
    {
        chaseAgent.StopChase();
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

    public void CanGetAxe()
    {
        axeOnGround = true;
        axeGroundPosition = bossAxe.transform.position;
        EndAttack();
    }
    #endregion

    #region Attacks

    public void DoEnragedAttack()
    {
        StartAttack();
        StartCoroutine(EnragedCharge());
    }

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
        bossAxe.StartThrowAttackA(transform.position + Vector3.up*0.5f, new Vector3(chaseAgent.TargetPosition().x - transform.position.x,0f, chaseAgent.TargetPosition().z - transform.position.z).normalized, axeOnAirTime);
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

    private IEnumerator EnragedCharge()
    {
        int count = 0;
        float timer;
        float maxTime;
        Vector3 dir, endPoint, startPoint;
        endPoint = Vector3.zero;
        Debug.Log("Enraged start");
        while (count < chargeAmount)
        {
            Debug.Log("loop start");
            timer = 0f;
            dir = chaseAgent.TargetDirection();
            startPoint = transform.position;
            Debug.Log("raycast start");
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 100f, wallMask))
            {
                endPoint = hit.point - dir * hb.radius;
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

            count++;
            DisableHitbox(enragedHitbox);
            yield return new WaitForSeconds(timeBetweenCharges);
        }
        recievedAxe = false;
        EndAttack();
        yield return null;
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

        while (timer < expandTime)
        {
            timer += Time.deltaTime;
            float t = timer / expandTime;
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
