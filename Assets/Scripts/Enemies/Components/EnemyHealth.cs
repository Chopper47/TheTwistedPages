using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    enum State{
        stay,
        chase,
        attack
    }

    State state = State.stay;

    private void Start()
    {
        health = maxHealth;
    }

    public void AttackState()
    {
        state = State.attack;
    }

    public void StayState()
    {
        state = State.stay;
    }

    public void ChaseState()
    {
        state = State.chase;
    }

    public int CurrentState()
    {
        return ((int)state);
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("Hole"))
        {
            Trap trap = other.GetComponent<Trap>();
            TakeDamage(trap.GetDamage());
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
