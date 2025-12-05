using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int health;

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

    public void Die()
    {
        if (roomController != null)
        {
            roomController.OnEnemyDied();
        }

        Destroy(gameObject);
    }
}
