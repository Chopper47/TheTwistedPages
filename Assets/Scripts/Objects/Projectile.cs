using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1;

    private Vector3 direction = Vector3.zero;

    private bool ignorePlayer = false;
    private bool wasDeflected = false;
    private float initialSize = 0.5f;

    private void Start()
    {
        initialSize = GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        if (direction != Vector3.zero && !wasDeflected)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    public void SetDirection(Vector3 newDir)
    {
        direction = newDir.normalized;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Deflect()
    {
        wasDeflected = true;
        GetComponent<SphereCollider>().radius = initialSize;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (wasDeflected && enemy && !ignorePlayer)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void IgnorePlayer()
    { 
        ignorePlayer = true;
    }
}
