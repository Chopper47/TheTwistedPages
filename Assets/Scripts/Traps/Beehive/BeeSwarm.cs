using UnityEngine;

public class BeeSwarm : MonoBehaviour
{
    private Transform target;
    private float duration;
    private float timer = 0f;

    [Header("Movement")]
    [SerializeField] private float followSpeed = 6.0f;
    [SerializeField] private float hoverHeight = 1.5f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageRate = 1.0f;

    private float damageTimer = 0f;

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.position + Vector3.up * hoverHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public void Initialize(Transform target, float duration)
    {
        this.target = target;
        this.duration = duration;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageRate)
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (other)
                {
                    player.TakeDamage(damage);
                }
                damageTimer = 0f;
            }
        }
    }
}
