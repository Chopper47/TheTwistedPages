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

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (roomController != null)
        {
            roomController.OnEnemyDied();
        }

        Destroy(gameObject);
    }
}
