using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int health;

    [HideInInspector] public RoomController roomController;

    private void Start()
    {
        health = maxHealth;

        roomController = GetComponentInParent<RoomController>();
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

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (roomController != null)
        {
            roomController.OnEnemyDied();
        }

        Destroy(gameObject);
    }
}
