using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5.0f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private int damage = 1;

    private float timer;

    private void Start()
    {
        timer = lifetime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
