using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    private void Start()
    {
        health = maxHealth;
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
