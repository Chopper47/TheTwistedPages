using UnityEngine;

public class FlowerProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1;

    private Vector3 direction = Vector3.zero;

    private bool wasDeflected = false;
    
    private void Update()
    {
        if (direction != Vector3.zero)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector3 newDir)
    {
        direction = newDir.normalized; 
    }

    public void Deflect(Vector3 newDir)
    {
        wasDeflected = true;
        SetDirection(newDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (wasDeflected && enemy)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
