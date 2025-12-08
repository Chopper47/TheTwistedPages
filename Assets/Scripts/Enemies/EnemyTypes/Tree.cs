using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [Header("General")]
    private int appleAttacksLeft;
    private bool isThrowing = false;
    private bool isSlamming = false;
    private bool canThrow = true;

    private Rigidbody rigidBody;
    private EnemyState enemyState;
    private ChaseAgent chaseAgent;
    private EnemyHealth enemyHealth;

    [Header("Apple Attack")]
    [SerializeField] private GameObject appleGrenadePrefab;
    [SerializeField] private int totalAppleAttacks = 3;
    [SerializeField] private int applesPerAttack = 8;
    [SerializeField] private float appleSpawnRadius = 4.0f;
    [SerializeField] private float appleAttackCooldown = 2.0f;

    [Header("Physical Attack")]
    [SerializeField] private float slamRadius = 4.0f;
    [SerializeField] private int slamDamage = 1;
    [SerializeField] private float slamDelay = 1.0f;


    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;
    private void Awake()
    {
        appleAttacksLeft = totalAppleAttacks;
        rigidBody = GetComponent<Rigidbody>();
        enemyState = GetComponent<EnemyState>();
        chaseAgent = GetComponent<ChaseAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
    }

    #region Apple Attack
    public bool HasApplesLeft()
    {
        return appleAttacksLeft > 0;
    }

    public bool IsThrowing()
    {
        return isThrowing;
    }

    public bool CanThrow()
    {
        return canThrow;
    }

    public void SetThrowing(bool throwing)
    {
        if (throwing && canThrow && appleAttacksLeft > 0)
        {
            StartCoroutine(PerformAppleAttack());
        }
    }

    private IEnumerator PerformAppleAttack()
    {
        isThrowing = true;
        canThrow = false;

        enemyState.AttackState();
        chaseAgent.StopChase();

        yield return new WaitForSeconds(0.5f);
        AM.Play("Creak",Main);
        for (int i = 0; i < applesPerAttack; i++)
        {
            float angle = i * Mathf.PI * 2.0f / applesPerAttack;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * appleSpawnRadius;
            Vector3 spawnPosition = transform.position + offset;

            spawnPosition.y = transform.position.y/2;

            Instantiate(appleGrenadePrefab, spawnPosition, Quaternion.identity);
        }

        appleAttacksLeft--;
        //Debug.Log($"Tree spawned {applesPerAttack} apples. Remaining apple attacks: {appleAttacksLeft}.");

        yield return new WaitForSeconds(appleAttackCooldown);

        isThrowing = false;
        canThrow = true;

        if (appleAttacksLeft <= 0)
        {
            //Debug.Log("Tree ran out of apple attacks.");
        }
    }
    #endregion

    #region Physical Attack
    public bool IsSlamming()
    {
        return isSlamming;
    }

    public void SetSlamming(bool slamming)
    {
        if (slamming && !isSlamming)
        {
            StartCoroutine(PerformPhysicalAttack());
        }
    }

    private IEnumerator PerformPhysicalAttack()
    {
        isSlamming = true;
        AM.Play("Crush", Main);
        chaseAgent.StopChase();
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        yield return null;

        enemyState.AttackState();

        yield return new WaitForSeconds(slamDelay);

        Collider[] hits = Physics.OverlapSphere(transform.position, slamRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.gameObject.GetComponent<PlayerHealth>();
                player.TakeDamage(slamDamage);
                Debug.Log("Tree hit the player with the physical attack.");
            }
        }

        enemyHealth.Die();
        
    }
    #endregion
}
