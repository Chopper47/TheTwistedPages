using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    public int health;

    private bool isDead = false;

    [HideInInspector] public RoomController roomController;

    [Header("VFX")]
    [SerializeField] private ParticleSystem deathParticles;

    [Header("SFX")]
    public string identity;
    [SerializeField] private AudioManager AM;
    public AudioSource Main;

    private void Start()
    {
        health = maxHealth;

        roomController = GetComponentInParent<RoomController>();

        Main = GetComponent<AudioSource>();
        AM = FindFirstObjectByType<AudioManager>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
            AM.Play(identity, null);
        }
    }

    public void SetHealth(int healthAmount)
    {
        health = healthAmount;

        if (health <= 0)
        {
            Die();
            AM.Play(identity, null);
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (deathParticles != null)
        {
            ParticleSystem particles = Instantiate(deathParticles, transform.position, Quaternion.identity);
            particles.Play();
        }

        if (roomController != null)
        {
            roomController.OnEnemyDied();
        }

        if (GetComponent<Lumberjack>())
        {
            GetComponent<Lumberjack>().enabled = false;
            GetComponent<ChaseAgent>().enabled = false;
            GetComponent<DecisionTreeAgent>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
