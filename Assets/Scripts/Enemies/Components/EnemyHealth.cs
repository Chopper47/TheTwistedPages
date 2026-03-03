using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    public int health;

    [HideInInspector] public RoomController roomController;
    [SerializeField] private AudioManager AM;
    public AudioSource Main;
    public string identity;
    
    private void Start()
    {
        health = maxHealth;

        roomController = GetComponentInParent<RoomController>();
        Main=GetComponent<AudioSource>();
        AM=FindFirstObjectByType<AudioManager>();
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
        //gameObject.SetActive(false);
        else
        {
            Destroy(gameObject);
        }

    }
}
